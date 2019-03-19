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
            HydroStatus currentStatus = new HydroStatusCalculator().GetCurrentStatus(hydroData, lastReportedStatus) ;

            _log.LogTrace($"Previous state: {lastReportedStatus}, current state: {currentStatus}");
            bool statusChanged = currentStatus != lastReportedStatus;
            if (statusChanged)
            {
                bool notificationsSent = SendNotifications(currentStatus, hydroData);

                if (notificationsSent)
                {
                    _stateService.SetStatus(currentStatus);
                }
            }
        }

        private bool SendNotifications(HydroStatus currentStatus, List<HydroData> data)
        {
            SendEmailNotification(currentStatus, data, _log);
            return SendSmsNotification(currentStatus, data, _log);
        }

        private void SendEmailNotification(HydroStatus currentStatus, List<HydroData> data, ILogger log)
        {
            var message = new EmailMessageBuilder(_settingsService)
                .BuildMessage(data, currentStatus, DateTime.Now);

            _messages.AddAsync(message);
        }

        private bool SendSmsNotification(HydroStatus currentStatus, List<HydroData> data, ILogger log)
        {
            var message = new SmsMessageBuilder()
                .BuildMessage(data, currentStatus, DateTime.Now, _settingsService.SmsTo);

            var smsNotifier = new SmsNotifier(_settingsService, _log);
            return smsNotifier.SendSmsNotification(message);
        }
    }
}
