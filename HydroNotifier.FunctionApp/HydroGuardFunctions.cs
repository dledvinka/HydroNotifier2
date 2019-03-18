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
            [SendGrid(
                To = "ledvinka.david@gmail.com",
                Subject = "Thank you!",
                Text = "Hi, Thank you for registering!!!!",
                From = "no-reply@jankowskimichal.pl")] out SendGridMessage message,
            ILogger log,
            ExecutionContext executionContext)
        {
            message = null;
            var path = executionContext.FunctionDirectory;

            try
            {
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                var hg = new HydroGuard(message, new StateService(path), log);
                Task.Run(() => hg.Do());
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, $"Exception found {ex.Message}");
            }
        }
    }
}
