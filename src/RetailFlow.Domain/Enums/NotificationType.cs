namespace RetailFlow.Domain.Enums
{
    public enum NotificationType
    {
        Email = 0,
        SMS = 1,
        Push = 2
    }

    public enum NotificationStatus
    {
        Pending = 0,
        Sent = 1,
        Failed = 2
    }
}
