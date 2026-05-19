using System;
using System.Collections.Generic;
using MongoDB.Driver;
using RetailFlow.Infrastructure.Mongo;
using Serilog;

namespace RetailFlow.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<NotificationDocument> _notifications;
        private static readonly ILogger _log = Log.ForContext<NotificationService>();

        public NotificationService(MongoDbContext mongo)
        {
            _notifications = mongo.GetCollection<NotificationDocument>("notifications");
        }

        public void SendEmail(string recipient, string subject, string message)
        {
            var doc = new NotificationDocument
            {
                Type = "EMAIL",
                Recipient = recipient,
                Subject = subject,
                Message = message,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            _notifications.InsertOne(doc);

            // Simulate sending (in production, integrate with SendGrid / Azure Communication Services)
            try
            {
                _log.Information("[EMAIL] To: {Recipient} | Subject: {Subject}", recipient, subject);
                doc.Status = "SENT";
                doc.SentAt = DateTime.UtcNow;
                _notifications.ReplaceOne(n => n.Id == doc.Id, doc);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to send email to {Recipient}", recipient);
                doc.Status = "FAILED";
                _notifications.ReplaceOne(n => n.Id == doc.Id, doc);
            }
        }

        public void SendSms(string recipient, string message)
        {
            var doc = new NotificationDocument
            {
                Type = "SMS",
                Recipient = recipient,
                Message = message,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            _notifications.InsertOne(doc);

            // Simulate SMS (in production, integrate with Twilio / Azure Communication Services)
            _log.Information("[SMS] To: {Recipient} | Message: {Message}", recipient, message);
            doc.Status = "SENT";
            doc.SentAt = DateTime.UtcNow;
            _notifications.ReplaceOne(n => n.Id == doc.Id, doc);
        }

        public void RetryFailedNotifications()
        {
            var failed = _notifications
                .Find(n => n.Status == "FAILED" && n.RetryCount < 3)
                .ToList();

            _log.Information("Retrying {Count} failed notifications", failed.Count);

            foreach (var n in failed)
            {
                try
                {
                    if (n.Type == "EMAIL")
                        _log.Information("[RETRY EMAIL] To: {Recipient}", n.Recipient);
                    else
                        _log.Information("[RETRY SMS] To: {Recipient}", n.Recipient);

                    n.Status = "SENT";
                    n.SentAt = DateTime.UtcNow;
                    n.RetryCount++;
                    _notifications.ReplaceOne(x => x.Id == n.Id, n);
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Retry failed for notification {Id}", n.Id);
                    n.RetryCount++;
                    _notifications.ReplaceOne(x => x.Id == n.Id, n);
                }
            }
        }
    }
}
