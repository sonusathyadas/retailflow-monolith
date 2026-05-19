using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RetailFlow.Infrastructure.Mongo
{
    public class NotificationDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }  // EMAIL, SMS

        [BsonElement("recipient")]
        public string Recipient { get; set; }

        [BsonElement("subject")]
        public string Subject { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "PENDING";  // PENDING, SENT, FAILED

        [BsonElement("retryCount")]
        public int RetryCount { get; set; } = 0;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("sentAt")]
        public DateTime? SentAt { get; set; }
    }
}
