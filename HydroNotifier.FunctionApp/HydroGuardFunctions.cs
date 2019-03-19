using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HydroNotifier.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace HydroNotifier.FunctionApp
{
    public static class HydroGuardFunctions
    {
        [FunctionName("HydroGuardTimerFunction")]
        public static void Run(
            //[TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
            [TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [SendGrid] out SendGridMessage message,
            ILogger log,
            ExecutionContext executionContext)
        {
            message = null;

            try
            {
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                var stateService = new StateService(executionContext.FunctionDirectory);
                var settingsService = new SettingsService();

                var hg = new HydroGuard(out message, stateService, settingsService, log);
                Task.Run(() => hg.Do());
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, $"Exception found {ex.Message}");
            }
        }
    }


}
