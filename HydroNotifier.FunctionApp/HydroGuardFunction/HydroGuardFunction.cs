using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using HydroNotifier.FunctionApp.Core;
using HydroNotifier.FunctionApp.Storage;
using HydroNotifier.FunctionApp.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace HydroNotifier.FunctionApp
{
    public static class HydroGuardFunction
    {
        [FunctionName("HydroGuardTimerFunction")]
        public static async void Run(
            [TimerTrigger("*/10 * * * * *")]TimerInfo myTimer,
            //[Table("HydroNotifierFlowData", Connection = "DefaultEndpointsProtocol=https;AccountName=hydronotifierfunctionapp;AccountKey=b6/bnGXZI3Gh1Jh5ScaKNboMAZCYbDwv/PYJpGemcOf31JlzjhgZOlVfqkJ2VioOIhKCNk5C8fZYZsnUMrxE9w==;EndpointSuffix=core.windows.net")]
            //[TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [SendGrid] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log,
            ExecutionContext executionContext)
        {
            try
            {
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                var tableService = TableService.Create();
                var stateService = new StateService(executionContext.FunctionDirectory);
                var settingsService = new SettingsService();
                var telemetry = new ApplicationInsightsTelemetry();

                var hg = new HydroGuard(tableService, messageCollector, stateService, settingsService, log, telemetry);
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
