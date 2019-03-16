using System;
using HydroNotifier.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace HydroNotifier.FunctionApp
{
    public static class HydroGuardFunctions
    {
        [FunctionName("HydroGuardTimerFunction")]
        public static async void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await new HydroGuard().Do(log);

            // https://medium.com/statuscode/getting-key-vault-secrets-in-azure-functions-37620fd20a0b
        }
    }
}
