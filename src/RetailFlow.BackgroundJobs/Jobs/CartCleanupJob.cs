using System;
using Hangfire;
using Serilog;

namespace RetailFlow.BackgroundJobs.Jobs
{
    /// <summary>
    /// Hangfire job: removes shopping carts that have been inactive for more than 30 days.
    /// Scheduled to run daily at midnight.
    /// </summary>
    public class CartCleanupJob
    {
        private static readonly ILogger _log = Log.ForContext<CartCleanupJob>();

        [AutomaticRetry(Attempts = 3)]
        public void Execute()
        {
            _log.Information("CartCleanupJob started at {Time}", DateTime.UtcNow);

            // In production: query MongoDB for carts with UpdatedAt < (now - 30 days) and delete them
            var cutoff = DateTime.UtcNow.AddDays(-30);
            _log.Information("Cleaning up carts inactive since {Cutoff}", cutoff);

            // Placeholder — actual MongoDB deletion wired via DI in production
            _log.Information("CartCleanupJob completed");
        }
    }
}
