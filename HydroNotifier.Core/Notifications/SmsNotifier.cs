namespace HydroNotifier.Core.Notifications;

using HydroNotifier.Core.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexmo.Api;
using Nexmo.Api.Request;

public class SmsNotifier
{
    private readonly ILogger _log;
    private readonly ISettingsService _settingsService;

    public SmsNotifier(ISettingsService settingsService, ILogger log)
    {
        _settingsService = settingsService;
        _log = log;
    }

    public string SendSmsNotification(SMS.SMSRequest message)
    {
        var smsClient = new Client(creds: new Credentials
        {
            ApiKey = _settingsService.SmsApiKey,
            ApiSecret = _settingsService.SmsApiSecret
        });

        var results = smsClient.SMS.Send(message);

        // {"message-count":"1","messages":[{"status":"0","message-id":"1500000011CDD9A6","to":"420735159055","client-ref":null,"remaining-balance":"10.86750000","message-price":"0.04530000","network":"23001","error-text":null}]}
        if (results.message_count != "1" || results.messages[0].status != "0")
            throw new InvalidOperationException($"Error during SMS send operation, result = {JsonConvert.SerializeObject(results)}");

        var remainingBalanceEur = results.messages[0].remaining_balance;

        return remainingBalanceEur;
    }
}