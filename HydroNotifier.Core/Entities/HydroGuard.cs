namespace HydroNotifier.Core.Entities;

using System.Text.Json;
using HydroNotifier.Core.Notifications;
using HydroNotifier.Core.Storage;
using HydroNotifier.Core.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Nexmo.Api;
using SendGrid.Helpers.Mail;
using Vonage.Messages.WhatsApp;
using Vonage;
using Vonage.Messages;
using Vonage.Messaging;
using Vonage.Request;

public class HydroGuard
{

    private readonly ILogger _log;
    private readonly IAsyncCollector<SendGridMessage> _messages;
    private readonly ISettingsService _settingsService;
    private readonly ITableService _tableService;
    private readonly ITelemetry _tc;

    public HydroGuard(
        ITableService tableService,
        IAsyncCollector<SendGridMessage> messages,
        ISettingsService settingsService,
        ILogger log,
        ITelemetry tc)
    {
        _tableService = tableService;
        _messages = messages;
        _settingsService = settingsService;
        _log = log;
        _tc = tc;
    }

    public async Task DoAsync()
    {
        string emailJson = null, smsJson = null, remainingBalanceEur = null;
        List<HydroData> hydroData = new List<HydroData>();

        using var httpClient = new HttpClient();
        hydroData.Add(await new WebScraper(HydroGuardQueries.LomnaQuery, httpClient, _log).GetLatestValuesAsync());
        hydroData.Add(await new WebScraper(HydroGuardQueries.OlseQuery, httpClient, _log).GetLatestValuesAsync());

        var lastReportedStatus = GetLastReportedStatus();
        _log.LogInformation($"Previous status: {lastReportedStatus}");
        var currentStatus = new HydroStatusCalculator(_tc).GetCurrentStatus(hydroData, lastReportedStatus);
        _log.LogInformation($"Current status: {currentStatus}");
        var statusChanged = currentStatus != lastReportedStatus;

        if (statusChanged)
        {
            _log.LogInformation($"Status change detected, sending notifications...");
            (emailJson, smsJson, remainingBalanceEur) = await SendNotificationsAsync(currentStatus, hydroData);
        }

        AddToStorage(hydroData, emailJson, smsJson, remainingBalanceEur, currentStatus);
    }

    private void AddToStorage(List<HydroData> hydroData, string emailJson, string smsJson, string nexmoRemainingBalanceEur, HydroStatus currentStatus)
    {
        var fde = new FlowDataEntity()
        {
            LomnaFlowLitersPerSecond = hydroData[0].FlowLitersPerSecond,
            OlseFlowLitersPerSecond = hydroData[1].FlowLitersPerSecond,
            EmailNotificationSent = !string.IsNullOrWhiteSpace(emailJson),
            EmailNotificationJson = emailJson,
            SmsNotificationSent = !string.IsNullOrWhiteSpace(smsJson),
            SmsNotificationJson = smsJson,
            NexmoRemainingBalanceEur = nexmoRemainingBalanceEur,
            Status = currentStatus.ToString(),
            Timestamp = DateTime.UtcNow
        };

        _tableService.InsertOrMergeAsync(fde);
    }

    private async Task<(string emailJson, string smsJson, string remainingBalanceEur)> SendNotificationsAsync(HydroStatus currentStatus, List<HydroData> data)
    {
        string emailJson, smsJson, remainingBalanceEur;

        try
        {
            emailJson = await SendEmailNotificationAsync(currentStatus, data);
            emailJson = string.Empty;
            _log.LogInformation("Email notification sent");
            (smsJson, remainingBalanceEur) = await SendSmsNotification2Async(currentStatus, data);
            _log.LogInformation("SMS notification sent");
            //var messageResponse = await SendWhatsAppNotificationAsync(currentStatus, data);
            //_log.LogInformation("WhatsApp notification sent");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error during sending notifications");
            throw;
        }

        return (emailJson, smsJson, remainingBalanceEur);
    }

    //private async Task<SendSmsResponse?> SendWhatsAppNotificationAsync(HydroStatus currentStatus, List<HydroData> data)
    //{
    //    //https://developer.vonage.com/en/messages/code-snippets/whatsapp/send-text?lang=dotnet
    //    var credentials = Credentials.FromApiKeyAndSecret("ab8748bb", "ji3GUysTpz60F3zW");

    //    var vonageClient = new VonageClient(credentials);

    //    var request = new SendSmsRequest()
    //    {
    //        From = "420735159055",
    //        To = "420797848833",
    //        Text = "A text message sent using the Vonage SMS API"
    //    };

    //    //var request = new WhatsAppTextRequest
    //    //{
    //    //    To = "420797848833",
    //    //    From = "420735159055",
    //    //    Text = "A WhatsApp text message sent using the Vonage Messages API"
    //    //};

    //    var response = await vonageClient.SmsClient.SendAnSmsAsync(request);

    //    return response;
    //}

    private async Task<string> SendEmailNotificationAsync(HydroStatus currentStatus, List<HydroData> data)
    {
        var message = new EmailMessageBuilder(_settingsService, _log)
            .BuildMessage(data, currentStatus, DateTime.Now);

        var jsonString = JsonSerializer.Serialize(message);
        _log.LogInformation($"Email message: {jsonString}");

        await _messages.AddAsync(message);

        return jsonString;
    }

    //private (string jsonString, string remainingBalanceEur) SendSmsNotification(HydroStatus currentStatus, List<HydroData> data)
    //{
    //    var message = new SmsMessageBuilder()
    //        .BuildMessage(data, currentStatus, DateTime.Now, _settingsService.SmsTo);

    //    var jsonString = JsonSerializer.Serialize(message);
    //    _log.LogInformation($"SMS message: {jsonString}");

    //    var smsNotifier = new SmsNotifier(_settingsService, _log);
    //    var remainingBalanceEur = smsNotifier.SendSmsNotification(message);

    //    return (jsonString, remainingBalanceEur);
    //}

    private async Task<(string jsonString, string remainingBalanceEur)> SendSmsNotification2Async(HydroStatus currentStatus, List<HydroData> data)
    {
        var message = new SmsMessageBuilder()
            .BuildMessage2(data, currentStatus, DateTime.Now, _settingsService.SmsTo);

        var jsonString = JsonSerializer.Serialize(message);
        _log.LogInformation($"SMS message: {jsonString}");

        var credentials = Credentials.FromApiKeyAndSecret(_settingsService.SmsApiKey, _settingsService.SmsApiSecret);
        var vonageClient = new VonageClient(credentials);

        var response = await vonageClient.SmsClient.SendAnSmsAsync(message);
        var remainingBalanceEur = response.Messages.FirstOrDefault()?.RemainingBalance ?? string.Empty;

        _log.LogInformation($"RemainingBalanceEur: {remainingBalanceEur}");

        return (jsonString, remainingBalanceEur);
    }

    private HydroStatus GetLastReportedStatus()
    {
        var lastSavedEntity = _tableService.GetLastOrDefault();

        if (lastSavedEntity != null)
        {
            if (Enum.TryParse(lastSavedEntity.Status, out HydroStatus status))
                return status;
        }

        return HydroStatus.Normal;
    }
}