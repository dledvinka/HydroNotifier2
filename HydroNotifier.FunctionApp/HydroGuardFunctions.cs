using System;
using HydroNotifier.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace HydroNotifier.FunctionApp
{
    public static class HydroGuardFunctions
    {
        private static string superSecret = System.Environment.GetEnvironmentVariable("TEST");

        [FunctionName("HydroGuardTimerFunction")]
        public static void Run(
            //[TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
            [TimerTrigger("0 0 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [SendGrid(
                To = "ledvinka.david@gmail.com",
                Subject = "Thank you!",
                Text = "Hi, Thank you for registering!!!!",
                From = "no-reply@jankowskimichal.pl")] out SendGridMessage message,
            ILogger log)
        {

            message = null;
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            new HydroGuard(message, log).Do();

            // https://medium.com/statuscode/getting-key-vault-secrets-in-azure-functions-37620fd20a0b
        }
    }
}
