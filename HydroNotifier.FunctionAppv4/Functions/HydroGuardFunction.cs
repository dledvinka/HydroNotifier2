namespace HydroNotifier.FunctionAppv4.Functions;

using System;
using System.Threading.Tasks;
using HydroNotifier.Core.Entities;
using HydroNotifier.Core.Storage;
using HydroNotifier.Core.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

public static class HydroGuardFunction
{
    [FunctionName("HydroGuardTimerFunction")]
    public static async Task Run(
        //[TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
        //[Table("HydroNotifierFlowData", Connection = "DefaultEndpointsProtocol=https;AccountName=hydronotifierfunctionapp;AccountKey=b6/bnGXZI3Gh1Jh5ScaKNboMAZCYbDwv/PYJpGemcOf31JlzjhgZOlVfqkJ2VioOIhKCNk5C8fZYZsnUMrxE9w==;EndpointSuffix=core.windows.net")]
        [TimerTrigger("0 0 * * * *", RunOnStartup = true)] TimerInfo myTimer, // hourly
        //[TimerTrigger("0 */5 * * * *")] TimerInfo myTime,
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
            throw;
        }
    }
}