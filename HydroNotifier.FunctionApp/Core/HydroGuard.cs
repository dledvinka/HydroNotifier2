using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HydroNotifier.FunctionApp.Notifications;
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
        private readonly IAsyncCollector<SendGridMessage> _messages;
        private readonly IStateService _stateService;
        private readonly ISettingsService _settingsService;

        public HydroGuard(IAsyncCollector<SendGridMessage> messages, IStateService stateService, ISettingsService settingsService, ILogger log)
        {
            _messages = messages;
            _stateService = stateService;
            _settingsService = settingsService;
            _log = log;
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
            HydroStatus currentStatus = new HydroStatusCalculator().GetCurrentStatus(hydroData, lastReportedStatus) ;
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
            var message = new EmailMessageBuilder(_settingsService)
                .BuildMessage(data, currentStatus, DateTime.Now);

            _log.LogInformation($"Email message: {message}");

            await _messages.AddAsync(message);
        }

        private void SendSmsNotification(HydroStatus currentStatus, List<HydroData> data)
        {
            var message = new SmsMessageBuilder()
                .BuildMessage(data, currentStatus, DateTime.Now, _settingsService.SmsTo);

            _log.LogInformation($"SMS message: {message}");

            var smsNotifier = new SmsNotifier(_settingsService, _log);
            smsNotifier.SendSmsNotification(message);
        }
    }
}
