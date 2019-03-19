using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace HydroNotifier.Core
{
    public class HydroGuard
    {
        private static HydroQuery _lomnaQuery = new HydroQuery("Lomná", "http://hydro.chmi.cz/hpps/popup_hpps_prfdyn.php?seq=307326");
        private static HydroQuery _olseQuery = new HydroQuery("Olše", "http://hydro.chmi.cz/hpps/popup_hpps_prfdyn.php?seq=307325");
        private ILogger _log;
        private SendGridMessage _message;
        private readonly IStateService _stateService;
        private readonly ISettingsService _settingsService;

        public HydroGuard(out SendGrid.Helpers.Mail.SendGridMessage message, IStateService stateService, ISettingsService settingsService, ILogger log)
        {
            _message = message = null;
            _stateService = stateService;
            _settingsService = settingsService;
            _log = log;
        }

        public async Task Do()
        {
            HydroData lomnaData, olseData;

            using (HttpClient client = new HttpClient())
            {
                lomnaData = await new WebScraper(_lomnaQuery, client, _log).GetLatestValuesAsync();
                olseData = await new WebScraper(_olseQuery, client, _log).GetLatestValuesAsync();
            }

            double flowSum = lomnaData.FlowLitresPerSecond + olseData.FlowLitresPerSecond;

            //double flowSum = 1000.0;
            //lomnaData = new HydroData("Reka1", DateTime.Now.ToString(), 1000.0);
            //olseData = new HydroData("Reka2", DateTime.Now.ToString(), 2000.0);

            var lastReportedStatus = _stateService.GetStatus();
            _log.LogInformation($"Last reported status: '{lastReportedStatus}'");
            HydroStatus currentStatus = GetCurrentStatus(flowSum, lastReportedStatus);

            bool statusChanged = currentStatus != lastReportedStatus;
            //if (statusChanged)
            {
                _log.LogInformation($"Status changed: '{currentStatus}'");
                SendNotifications(currentStatus, lomnaData, olseData);

                _stateService.SetStatus(currentStatus);
                _log.LogInformation($"New status saved: '{currentStatus}'");
            }

            _log.LogInformation("Done.");
        }

        private void SendNotifications(HydroStatus currentStatus, HydroData lomnaData, HydroData olseData)
        {
            SendEmailNotification(currentStatus, lomnaData, olseData, _log);
            SendSmsNotification(currentStatus, lomnaData, olseData, _log);
        }

        private void SendEmailNotification(HydroStatus currentStatus, HydroData lomnaData, HydroData olseData, ILogger log)
        {
            var message = new EmailMessageBuilder(_settingsService)
                .BuildMessage(lomnaData, olseData, currentStatus, DateTime.Now);

            _message = message;
        }

        private void SendSmsNotification(HydroStatus currentStatus, HydroData lomnaData, HydroData olseData, ILogger log)
        {
            var message = new SmsMessageBuilder()
                .BuildMessage(lomnaData, olseData, currentStatus, DateTime.Now, _settingsService.SmsTo);

            var smsNotifier = new SmsNotifier(_settingsService, _log);
            smsNotifier.SendSmsNotification(message);
        }

        private HydroStatus GetCurrentStatus(double flowSum, HydroStatus lastReportedStatus)
        {
            const double NORMAL_TO_LOW_THRESHOLD = 630.0;
            const double LOW_TO_NORMAL_THRESHOLD = 650.0;
            const double NORMAL_TO_HIGH_THRESHOLD = 20000.0;
            const double HIGH_TO_NORMAL_THRESHOLD = 13000.0;

            if (lastReportedStatus != HydroStatus.Low && flowSum <= NORMAL_TO_LOW_THRESHOLD)
                return HydroStatus.Low;

            if (lastReportedStatus != HydroStatus.High && flowSum >= NORMAL_TO_HIGH_THRESHOLD)
                return HydroStatus.High;

            if (lastReportedStatus != HydroStatus.Normal && flowSum > LOW_TO_NORMAL_THRESHOLD && flowSum < HIGH_TO_NORMAL_THRESHOLD)
                return HydroStatus.Normal;

            return lastReportedStatus;
        }
    }
}
