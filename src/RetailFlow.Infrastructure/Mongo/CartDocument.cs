using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RetailFlow.Infrastructure.Mongo
{
    public class CartDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        public int UserId { get; set; }

        [BsonElement("items")]
        public List<CartItemDocument> Items { get; set; } = new List<CartItemDocument>();

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CartItemDocument
    {
        [BsonElement("productId")]
        public string ProductId { get; set; }

        [BsonElement("productName")]
        public string ProductName { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }
    }
}
