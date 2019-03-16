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

        public async Task Do(ILogger log)
        {
            HydroData lomnaData, olseData;

            using (HttpClient client = new HttpClient())
            {
                lomnaData = await new WebScraper(_lomnaQuery, client, log).GetLatestValuesAsync();
                olseData = await new WebScraper(_olseQuery, client, log).GetLatestValuesAsync();
            }

            double flowSum = lomnaData.FlowLitresPerSecond + olseData.FlowLitresPerSecond;

            HydroStatus currentStatus = GetCurrentStatus(flowSum, _lastReportedStatus);

            if (currentStatus != _lastReportedStatus)
            {
                OnStatusChanged(currentStatus, lomnaData, olseData, log);
            }

            log.LogInformation("Done.");
        }

        private void OnStatusChanged(HydroStatus currentStatus, HydroData lomnaData, HydroData olseData, ILogger log)
        {
            log.LogInformation($"Status changed: '{currentStatus}'");
            _lastReportedStatus = currentStatus;

            //SendSmsNotification(log);
            //SendEmailNotification(log);
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
