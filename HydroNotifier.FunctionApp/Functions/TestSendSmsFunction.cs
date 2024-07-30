namespace HydroNotifier.FunctionApp.Functions;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using HydroNotifier.Core.Entities;
using HydroNotifier.Core.Notifications;
using HydroNotifier.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

public static class TestSendSmsFunction
{
    //[FunctionName("TestSendSmsFunction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
        HttpRequest req,
        [SendGrid] IAsyncCollector<SendGridMessage> messageCollector,
        ILogger log)
    {
        try
        {
            log.LogInformation("HttpTrigger => TestSendSmsFunction");
            var settingsService = new SettingsService();

            var data = new List<HydroData>()
            {
                new HydroData("TestRiverName1", DateTime.UtcNow.ToString(), 50.0),
                new HydroData("TestRiverName2", DateTime.UtcNow.ToString(), 60.0),
            };

            //var flowData = tableService.GetAll<FlowDataEntity>().OrderByDescending(e => e.Timestamp);
            var currentStatus = HydroStatus.Normal;

            var message = new EmailMessageBuilder(settingsService, log)
                .BuildMessage(data, currentStatus, DateTime.Now);

            var jsonString = JsonSerializer.Serialize(message);
            log.LogInformation($"Email message: {jsonString}");

            await messageCollector.AddAsync(message);
            await messageCollector.FlushAsync();

            var smsMessage = new SmsMessageBuilder()
                .BuildMessage(data, currentStatus, DateTime.Now, settingsService.SmsTo);

            jsonString = JsonSerializer.Serialize(smsMessage);
            log.LogInformation($"SMS message: {jsonString}");

            var smsNotifier = new SmsNotifier(settingsService, log);
            smsNotifier.SendSmsNotification(smsMessage);

            var responseMessage = "Both notifications sent";

            return new OkObjectResult(responseMessage);
        }
        catch (Exception ex)
        {
            log.LogCritical(ex, $"Exception found {ex.Message}");
            return new BadRequestResult();
        }
    }
}