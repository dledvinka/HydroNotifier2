using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexmo.Api;

namespace FunctionApp2
{
    public static class Function1
    {
        private static HydroQuery _lomnaQuery = new HydroQuery("Lomná", "http://hydro.chmi.cz/hpps/popup_hpps_prfdyn.php?seq=307326");
        private static HydroQuery _olseQuery = new HydroQuery("Olše", "http://hydro.chmi.cz/hpps/popup_hpps_prfdyn.php?seq=307325");
        private static HydroStatus _lastReportedStatus = HydroStatus.Unknown;

        [FunctionName("Function1")]
        public static async void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await Do(log);

            // https://medium.com/statuscode/getting-key-vault-secrets-in-azure-functions-37620fd20a0b
        }

        private static async Task Do(ILogger log)
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
        }

        private static void OnStatusChanged(HydroStatus currentStatus, HydroData lomnaData, HydroData olseData, ILogger log)
        {
            log.LogInformation($"Status changed: '{currentStatus}'");
            _lastReportedStatus = currentStatus;

            //SendSmsNotification(log);
            //SendEmailNotification(log);
        }

        private static void SendEmailNotification(ILogger log)
        {
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("", "");
                client.Port = 587;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("hydronotifier@noreply.com");
                    mailMessage.To.Add("");
                    mailMessage.Body = "body";
                    mailMessage.Subject = "HydroNotifier";
                    client.Send(mailMessage);
                }
            }
        }

        private static void SendSmsNotification(ILogger log)
        {
            var smsClient = new Client(creds: new Nexmo.Api.Request.Credentials
            {
                ApiKey = "",
                ApiSecret = ""
            });

            var results = smsClient.SMS.Send(request: new SMS.SMSRequest
            {
                from = "HydroNotifier",
                to = "",
                text = "A test SMS sent using the Nexmo SMS API"
            });

            log.LogInformation(JsonConvert.SerializeObject(results));
        }

        private static HydroStatus GetCurrentStatus(double flowSum, HydroStatus lastReportedStatus)
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
