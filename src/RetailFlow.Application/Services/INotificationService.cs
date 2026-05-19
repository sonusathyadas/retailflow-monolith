namespace RetailFlow.Application.Services
{
    public interface INotificationService
    {
        void SendEmail(string recipient, string subject, string message);
        void SendSms(string recipient, string message);
        void RetryFailedNotifications();
    }
}
