﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HydroNotifier.FunctionApp.Notifications;
using HydroNotifier.FunctionApp.Storage;
using HydroNotifier.FunctionApp.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace HydroNotifier.FunctionApp.Core
{
    public class HydroGuard
    {
        private static readonly HydroQuery _lomnaQuery = new HydroQuery("Lomná", "http://hydro.chmi.cz/hpps/popup_hpps_prfdyn.php?seq=307326");
        private static readonly HydroQuery _olseQuery = new HydroQuery("Olše", "http://hydro.chmi.cz/hpps/popup_hpps_prfdyn.php?seq=307325");
        private readonly ILogger _log;
        private readonly ITelemetry _tc;
        private readonly ITableService _tableService;
        private readonly IAsyncCollector<SendGridMessage> _messages;
        private readonly ISettingsService _settingsService;

        public HydroGuard(
            ITableService tableService, 
            IAsyncCollector<SendGridMessage> messages, 
            ISettingsService settingsService, 
            ILogger log, 
            ITelemetry tc)
        {
            _tableService = tableService;
            _messages = messages;
            _settingsService = settingsService;
            _log = log;
            _tc = tc;
        }

        public async Task DoAsync()
        {
            string emailJson = null, smsJson = null, remainingBalanceEur = null;
            List<HydroData> hydroData = new List<HydroData>();

            using (HttpClient client = new HttpClient())
            {
                hydroData.Add(await new WebScraper(_lomnaQuery, client, _log).GetLatestValuesAsync());
                hydroData.Add(await new WebScraper(_olseQuery, client, _log).GetLatestValuesAsync());
            }

            var lastReportedStatus = GetLastReportedStatus();
            _log.LogInformation($"Previous status: {lastReportedStatus}");
            HydroStatus currentStatus = new HydroStatusCalculator(_tc).GetCurrentStatus(hydroData, lastReportedStatus) ;
            _log.LogInformation($"Current status: {currentStatus}");
            bool statusChanged = currentStatus != lastReportedStatus;

            if (statusChanged)
            {
                _log.LogInformation($"Status change detected, sending notifications...");
                (emailJson, smsJson, remainingBalanceEur) = await SendNotificationsAsync(currentStatus, hydroData);
            }

            AddToStorage(hydroData, emailJson, smsJson, remainingBalanceEur, currentStatus);
        }

        private void AddToStorage(List<HydroData> hydroData, string emailJson, string smsJson, string nexmoRemainingBalanceEur, HydroStatus currentStatus)
        {
            var fde = new FlowDataEntity()
            {
                LomnaFlowLitersPerSecond = hydroData[0].FlowLitersPerSecond,
                OlseFlowLitersPerSecond = hydroData[1].FlowLitersPerSecond,
                EmailNotificationSent = !string.IsNullOrWhiteSpace(emailJson),
                EmailNotificationJson = emailJson,
                SmsNotificationSent = !string.IsNullOrWhiteSpace(smsJson),
                SmsNotificationJson = smsJson,
                NexmoRemainingBalanceEur = nexmoRemainingBalanceEur,
                Status = currentStatus.ToString(),
                Timestamp = DateTime.UtcNow
            };

            _tableService.InsertOrMergeAsync(fde);
        }

        private async Task<(string emailJson, string smsJson, string remainingBalanceEur)> SendNotificationsAsync(HydroStatus currentStatus, List<HydroData> data)
        {
            string emailJson, smsJson, remainingBalanceEur;
            
            try
            {
                emailJson = await SendEmailNotificationAsync(currentStatus, data);
                _log.LogInformation("Email notification sent");
                (smsJson, remainingBalanceEur) = SendSmsNotification(currentStatus, data);
                _log.LogInformation("SMS notification sent");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error during sending notifications");
                return (string.Empty, string.Empty, string.Empty);
            }

            return (emailJson, smsJson, remainingBalanceEur);
        }

        private async Task<string> SendEmailNotificationAsync(HydroStatus currentStatus, List<HydroData> data)
        {
            var message = new EmailMessageBuilder(_settingsService, _log)
                .BuildMessage(data, currentStatus, DateTime.Now);

            string jsonString = JsonSerializer.Serialize(message);
            _log.LogInformation($"Email message: {jsonString}");

            await _messages.AddAsync(message);
            
            return jsonString;
        }

        private (string jsonString, string remainingBalanceEur) SendSmsNotification(HydroStatus currentStatus, List<HydroData> data)
        {
            var message = new SmsMessageBuilder()
                .BuildMessage(data, currentStatus, DateTime.Now, _settingsService.SmsTo);

            string jsonString = JsonSerializer.Serialize(message);
            _log.LogInformation($"SMS message: {jsonString}");

            var smsNotifier = new SmsNotifier(_settingsService, _log);
            string remainingBalanceEur = smsNotifier.SendSmsNotification(message);

            return (jsonString, remainingBalanceEur);
        }

        private HydroStatus GetLastReportedStatus()
        {
            var lastSavedEntity = _tableService.GetLastOrDefault();

            if (lastSavedEntity != null)
            {
                if (Enum.TryParse(lastSavedEntity.Status, out HydroStatus status))
                {
                    return status;
                }
            }

            return HydroStatus.Normal;
        }
    }
}
