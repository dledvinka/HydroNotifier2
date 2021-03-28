using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HydroNotifier.FunctionApp.Core;
using HydroNotifier.FunctionApp.Notifications;
using HydroNotifier.FunctionApp.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;

namespace HydroNotifier.FunctionApp.Functions
{
    public static class TestSendSmsFunction
    {
        [FunctionName("TestSendSmsFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [SendGrid] IAsyncCollector<SendGridMessage> messageCollector,
            ExecutionContext executionContext,
            ILogger log)
        {
            try
            {
                log.LogInformation("HttpTrigger => TestSendSmsFunction");
                var settingsService = new SettingsService();

                List<HydroData> data = new List<HydroData>()
            {
                new HydroData("TestRiverName1", DateTime.UtcNow.ToString(), 50.0M),
                new HydroData("TestRiverName2", DateTime.UtcNow.ToString(), 60.0M),
            };

                var currentStatus = HydroStatus.Normal;

                var message = new EmailMessageBuilder(settingsService)
                    .BuildMessage(data, currentStatus, DateTime.Now);

                log.LogInformation($"Email message: {message}");

                await messageCollector.AddAsync(message);
                await messageCollector.FlushAsync();

                var smsMessage = new SmsMessageBuilder()
                    .BuildMessage(data, currentStatus, DateTime.Now, settingsService.SmsTo);

                log.LogInformation($"SMS message: {message}");

                var smsNotifier = new SmsNotifier(settingsService, log);
                smsNotifier.SendSmsNotification(smsMessage);

                string responseMessage = "Both notifications sent";

                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, $"Exception found {ex.Message}");
                return new BadRequestResult();
            }
        }
    }
}

