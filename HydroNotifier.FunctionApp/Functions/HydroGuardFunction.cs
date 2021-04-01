using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HydroNotifier.FunctionApp.Core;
using HydroNotifier.FunctionApp.Storage;
using HydroNotifier.FunctionApp.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace HydroNotifier.FunctionApp.Functions
{
    public static class HydroGuardFunction
    {
        [FunctionName("HydroGuardTimerFunction")]
        public static async void Run(
            //[TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
            //[Table("HydroNotifierFlowData", Connection = "DefaultEndpointsProtocol=https;AccountName=hydronotifierfunctionapp;AccountKey=b6/bnGXZI3Gh1Jh5ScaKNboMAZCYbDwv/PYJpGemcOf31JlzjhgZOlVfqkJ2VioOIhKCNk5C8fZYZsnUMrxE9w==;EndpointSuffix=core.windows.net")]
            [TimerTrigger("0 0 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [SendGrid] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            try
            {
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                var settingsService = new SettingsService();
                var tableService = new TableService(settingsService);
                var telemetry = new ApplicationInsightsTelemetry();

                var hg = new HydroGuard(tableService, messageCollector, settingsService, log, telemetry);
                await hg.DoAsync();
                await messageCollector.FlushAsync();
                log.LogInformation("Done.");
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, $"Exception found {ex.Message}");
            }
        }
    }
}
