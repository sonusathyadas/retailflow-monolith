using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RetailFlow.Infrastructure.Mongo
{
    /// <summary>
    /// MongoDB document for the Product Catalog.
    /// Uses flexible schema to support dynamic product attributes.
    /// </summary>
    public class ProductDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("attributes")]
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        [BsonElement("images")]
        public List<string> Images { get; set; } = new List<string>();

        [BsonElement("inventoryCount")]
        public int InventoryCount { get; set; }

        [BsonElement("rating")]
        public double Rating { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
