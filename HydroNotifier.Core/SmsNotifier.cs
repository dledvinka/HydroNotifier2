using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.Core
{
    public class SmsNotifier
    {
        public void SendSmsNotification(ILogger log)
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
    }
}
