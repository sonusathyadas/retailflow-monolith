using System;
using Hangfire;
using Serilog;

namespace RetailFlow.BackgroundJobs.Jobs
{
    /// <summary>
    /// Hangfire job: generates daily sales and inventory reports.
    /// Scheduled to run daily at 1 AM.
    /// </summary>
    public class ReportGenerationJob
    {
        private static readonly ILogger _log = Log.ForContext<ReportGenerationJob>();

        [AutomaticRetry(Attempts = 2)]
        public void Execute()
        {
            var reportDate = DateTime.UtcNow.Date.AddDays(-1);
            _log.Information("ReportGenerationJob generating report for {Date}", reportDate);

            // In production: aggregate orders, payments, inventory data and persist to a reports store
            _log.Information("ReportGenerationJob completed for {Date}", reportDate);
        }
    }
}
