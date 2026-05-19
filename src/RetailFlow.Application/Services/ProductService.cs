using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using RetailFlow.Application.DTOs;
using RetailFlow.Infrastructure.Mongo;
using RetailFlow.Infrastructure.Redis;
using Serilog;

namespace RetailFlow.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly MongoDbContext _mongo;
        private readonly ICacheService _cache;
        private readonly IMongoCollection<ProductDocument> _products;
        private static readonly ILogger _log = Log.ForContext<ProductService>();
        private const string CachePrefix = "product:";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

        public ProductService(MongoDbContext mongo, ICacheService cache)
        {
            _mongo = mongo;
            _cache = cache;
            _products = mongo.GetCollection<ProductDocument>("products");
        }

        public ProductDto GetById(string id)
        {
            // Try cache first
            var cached = _cache.Get<ProductDto>(CachePrefix + id);
            if (cached != null) return cached;

            var doc = _products.Find(p => p.Id == id && p.IsActive).FirstOrDefault();
            if (doc == null) return null;

            var dto = MapToDto(doc);
            _cache.Set(CachePrefix + id, dto, CacheTtl);
            return dto;
        }

        public IEnumerable<ProductDto> Search(ProductSearchRequest request)
        {
            var builder = Builders<ProductDocument>.Filter;
            var filter = builder.Eq(p => p.IsActive, true);

            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                var textFilter = builder.Or(
                    builder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(request.Query, "i")),
                    builder.Regex(p => p.Description, new MongoDB.Bson.BsonRegularExpression(request.Query, "i"))
                );
                filter = builder.And(filter, textFilter);
            }

            if (!string.IsNullOrWhiteSpace(request.Category))
                filter = builder.And(filter, builder.Eq(p => p.Category, request.Category));

            if (request.MinPrice.HasValue)
                filter = builder.And(filter, builder.Gte(p => p.Price, request.MinPrice.Value));

            if (request.MaxPrice.HasValue)
                filter = builder.And(filter, builder.Lte(p => p.Price, request.MaxPrice.Value));

            var skip = (request.Page - 1) * request.PageSize;
            var docs = _products.Find(filter).Skip(skip).Limit(request.PageSize).ToList();

            return docs.Select(MapToDto);
        }

        public ProductDto Create(CreateProductRequest request)
        {
            var doc = new ProductDocument
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Price = request.Price,
                Attributes = request.Attributes,
                Images = request.Images,
                InventoryCount = request.InventoryCount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _products.InsertOne(doc);
            _log.Information("Product created: {ProductId} - {Name}", doc.Id, doc.Name);
            return MapToDto(doc);
        }

        public ProductDto Update(string id, UpdateProductRequest request)
        {
            var doc = _products.Find(p => p.Id == id).FirstOrDefault();
            if (doc == null) throw new KeyNotFoundException($"Product {id} not found.");

            var update = Builders<ProductDocument>.Update
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            if (!string.IsNullOrWhiteSpace(request.Name))
                update = update.Set(p => p.Name, request.Name);
            if (!string.IsNullOrWhiteSpace(request.Description))
                update = update.Set(p => p.Description, request.Description);
            if (!string.IsNullOrWhiteSpace(request.Category))
                update = update.Set(p => p.Category, request.Category);
            if (request.Price.HasValue)
                update = update.Set(p => p.Price, request.Price.Value);
            if (request.Attributes != null)
                update = update.Set(p => p.Attributes, request.Attributes);
            if (request.Images != null)
                update = update.Set(p => p.Images, request.Images);

            _products.UpdateOne(p => p.Id == id, update);
            _cache.Remove(CachePrefix + id);

            return GetById(id);
        }

        public void Delete(string id)
        {
            // Soft delete
            var update = Builders<ProductDocument>.Update
                .Set(p => p.IsActive, false)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            _products.UpdateOne(p => p.Id == id, update);
            _cache.Remove(CachePrefix + id);
            _log.Information("Product soft-deleted: {ProductId}", id);
        }

        private static ProductDto MapToDto(ProductDocument doc)
        {
            return new ProductDto
            {
                Id = doc.Id,
                Name = doc.Name,
                Description = doc.Description,
                Category = doc.Category,
                Price = doc.Price,
                Attributes = doc.Attributes,
                Images = doc.Images,
                InventoryCount = doc.InventoryCount,
                Rating = doc.Rating
            };
        }
    }
}
