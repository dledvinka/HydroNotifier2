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
        private readonly IStateService _stateService;
        private readonly ISettingsService _settingsService;

        public HydroGuard(ITableService tableService, IAsyncCollector<SendGridMessage> messages, IStateService stateService,
            ISettingsService settingsService, ILogger log, ITelemetry tc)
        {
            _tableService = tableService;
            _messages = messages;
            _stateService = stateService;
            _settingsService = settingsService;
            _log = log;
            _tc = tc;
        }

        public async Task DoAsync()
        {
            List<HydroData> hydroData = new List<HydroData>();

            using (HttpClient client = new HttpClient())
            {
                hydroData.Add(await new WebScraper(_lomnaQuery, client, _log).GetLatestValuesAsync());
                hydroData.Add(await new WebScraper(_olseQuery, client, _log).GetLatestValuesAsync());
            }

            var lastReportedStatus = _stateService.GetStatus();
            _log.LogInformation($"Previous status: {lastReportedStatus}");
            HydroStatus currentStatus = new HydroStatusCalculator(_tc).GetCurrentStatus(hydroData, lastReportedStatus) ;
            _log.LogInformation($"Current status: {currentStatus}");
            bool statusChanged = currentStatus != lastReportedStatus;
            if (statusChanged)
            {
                _log.LogInformation($"Status change detected, sending notifications...");
                bool notificationsSent = await SendNotificationsAsync(currentStatus, hydroData);

                if (notificationsSent)
                {
                    _stateService.SetStatus(currentStatus);
                    _log.LogInformation($"Persisted status set to: {currentStatus}");
                }
            }

            AddToStorage(hydroData);
        }

        private void AddToStorage(List<HydroData> hydroData)
        {
            var fde = new FlowDataEntity()
                {
                    PartitionKey = "P",
                    RowKey = Guid.NewGuid().ToString(),
                    LomnaFlowLitersPerSecond = hydroData[0].FlowLitersPerSecond,
                    OlseFlowLitersPerSecond = hydroData[1].FlowLitersPerSecond,
                    
                    RiverName = hd.RiverName,
                    Timestamp = DateTime.UtcNow,
                    FlowLitersPerSecond = hd.FlowLitersPerSecond
                };

                _tableService.AddEntity(fde);
            }
        }

        private async Task<bool> SendNotificationsAsync(HydroStatus currentStatus, List<HydroData> data)
        {
            try
            {
                await SendEmailNotificationAsync(currentStatus, data);
                _log.LogInformation("Email notification sent");
                SendSmsNotification(currentStatus, data);
                _log.LogInformation("SMS notification sent");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error during sending notifications");
                return false;
            }

            return true;
        }

        private async Task SendEmailNotificationAsync(HydroStatus currentStatus, List<HydroData> data)
        {
            var message = new EmailMessageBuilder(_settingsService, _log)
                .BuildMessage(data, currentStatus, DateTime.Now);

            string jsonString = JsonSerializer.Serialize(message);
            _log.LogInformation($"Email message: {jsonString}");

            await _messages.AddAsync(message);
        }

        private void SendSmsNotification(HydroStatus currentStatus, List<HydroData> data)
        {
            var message = new SmsMessageBuilder()
                .BuildMessage(data, currentStatus, DateTime.Now, _settingsService.SmsTo);

            string jsonString = JsonSerializer.Serialize(message);
            _log.LogInformation($"SMS message: {jsonString}");

            var smsNotifier = new SmsNotifier(_settingsService, _log);
            smsNotifier.SendSmsNotification(message);
        }
    }
}
