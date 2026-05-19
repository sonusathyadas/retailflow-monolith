using System.Configuration;
using Hangfire;
using RetailFlow.BackgroundJobs.Jobs;

namespace RetailFlow.API.App_Start
{
    public static class HangfireConfig
    {
        public static void Configure()
        {
            var connStr = ConfigurationManager.ConnectionStrings["RetailFlowDb"].ConnectionString;

            GlobalConfiguration.Configuration
                .UseSqlServerStorage(connStr)
                .UseSerilogLogProvider();

            // ── Recurring Jobs ────────────────────────────────────────────────
            // Daily at midnight UTC
            RecurringJob.AddOrUpdate<CartCleanupJob>(
                "cart-cleanup",
                job => job.Execute(),
                Cron.Daily);

            // Daily at 1 AM UTC
            RecurringJob.AddOrUpdate<ReportGenerationJob>(
                "report-generation",
                job => job.Execute(),
                "0 1 * * *");

            // Every 15 minutes
            RecurringJob.AddOrUpdate<RetryNotificationsJob>(
                "retry-notifications",
                job => job.Execute(),
                "*/15 * * * *");
        }
    }
}
