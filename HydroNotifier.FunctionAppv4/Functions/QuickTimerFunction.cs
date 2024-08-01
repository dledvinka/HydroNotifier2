using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace HydroNotifier.FunctionApp.Functions
{
    public static class QuickTimerFunction
    {
        [FunctionName("QuickTimerFunction")]
        public static void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"QuickTimerFunction: C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}

