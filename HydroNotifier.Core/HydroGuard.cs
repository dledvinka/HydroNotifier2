using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HydroNotifier.Core
{
    public class HydroGuard
    {
        private static HydroQuery _lomnaQuery = new HydroQuery("Lomná", "http://hydro.chmi.cz/hpps/popup_hpps_prfdyn.php?seq=307326");
        private static HydroQuery _olseQuery = new HydroQuery("Olše", "http://hydro.chmi.cz/hpps/popup_hpps_prfdyn.php?seq=307325");
        private static HydroStatus _lastReportedStatus = HydroStatus.Unknown;
        private SendGrid.Helpers.Mail.SendGridMessage _message;
        private ILogger _log;

        public HydroGuard(SendGrid.Helpers.Mail.SendGridMessage message, ILogger log)
        {
            _message = message;
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

            HydroStatus currentStatus = GetCurrentStatus(flowSum, _lastReportedStatus);

            if (currentStatus != _lastReportedStatus)
            {
                OnStatusChanged(currentStatus, lomnaData, olseData, _log);
            }

            _log.LogInformation("Done.");
        }

        private void OnStatusChanged(HydroStatus currentStatus, HydroData lomnaData, HydroData olseData, ILogger log)
        {
            log.LogInformation($"Status changed: '{currentStatus}'");

            SendEmailNotification(log);
            SendSmsNotification(log);

            _lastReportedStatus = currentStatus;
        }

        private void SendEmailNotification(ILogger log)
        {
            _message = new SendGrid.Helpers.Mail.SendGridMessage();
        }

        private void SendSmsNotification(ILogger log)
        {
            var smsNotifier = new SmsNotifier(new SettingsService(), _log);
            smsNotifier.SendSmsNotification("TEST HydroNorifier");
        }

        private HydroStatus GetCurrentStatus(double flowSum, HydroStatus lastReportedStatus)
        {
            const double NORMAL_TO_LOW_THRESHOLD = 630.0;
            const double LOW_TO_NORMAL_THRESHOLD = 650.0;
            const double NORMAL_TO_HIGH_THRESHOLD = 20000.0;
            const double HIGH_TO_NORMAL_THRESHOLD = 16000.0;

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
