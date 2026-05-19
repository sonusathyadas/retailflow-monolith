using System;
using Hangfire;
using RetailFlow.Application.Services;
using Serilog;

namespace RetailFlow.BackgroundJobs.Jobs
{
    /// <summary>
    /// Hangfire job: retries failed notifications every 15 minutes.
    /// </summary>
    public class RetryNotificationsJob
    {
        private readonly INotificationService _notificationService;
        private static readonly ILogger _log = Log.ForContext<RetryNotificationsJob>();

        public RetryNotificationsJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [AutomaticRetry(Attempts = 2)]
        public void Execute()
        {
            _log.Information("RetryNotificationsJob started at {Time}", DateTime.UtcNow);
            _notificationService.RetryFailedNotifications();
            _log.Information("RetryNotificationsJob completed");
        }
    }
}
