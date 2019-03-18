using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexmo.Api;

namespace HydroNotifier.Core
{
    public class SmsNotifier
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogger _log;

        public SmsNotifier(ISettingsService settingsService, ILogger log)
        {
            _settingsService = settingsService;
            _log = log;
        }

        public void SendSmsNotification(string message)
        {
            var smsClient = new Nexmo.Api.Client(creds: new Nexmo.Api.Request.Credentials
            {
                ApiKey = _settingsService.SmsApiKey,
                ApiSecret = _settingsService.SmsApiSecret
            });

            var results = smsClient.SMS.Send(request: new SMS.SMSRequest
            {
                from = "HydroNotifier",
                to = _settingsService.SmsTo,
                text = message
            });

            // {"message-count":"1","messages":[{"status":"0","message-id":"1500000011CDD9A6","to":"420735159055","client-ref":null,"remaining-balance":"10.86750000","message-price":"0.04530000","network":"23001","error-text":null}]}
            _log.LogInformation(JsonConvert.SerializeObject(results));
        }
    }
}
