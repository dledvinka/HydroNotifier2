using System;
using HydroNotifier.FunctionApp.Core;
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
            //[TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
            [TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [SendGrid] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log,
            ExecutionContext executionContext)
        {
            try
            {
                log.LogTrace($"C# Timer trigger function executed at: {DateTime.Now}");
                var stateService = new StateService(executionContext.FunctionDirectory);
                var settingsService = new SettingsService();

                var hg = new HydroGuard(messageCollector, stateService, settingsService, log);
                await hg.DoAsync();
                await messageCollector.FlushAsync();
                log.LogTrace("Done.");
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, $"Exception found {ex.Message}");
            }
        }
    }


}
