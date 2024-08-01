namespace HydroNotifier.Core.Notifications;

using HydroNotifier.Core.Entities;
using HydroNotifier.Core.Utils;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using Convert = HydroNotifier.Core.Utils.Convert;

public class EmailMessageBuilder
{
    private readonly ILogger _log;
    private readonly ISettingsService _settingsService;

    public EmailMessageBuilder(ISettingsService settingsService, ILogger log)
    {
        _settingsService = settingsService;
        _log = log;
    }

    public SendGridMessage BuildMessage(List<HydroData> data, HydroStatus currentStatus, DateTime stateChangedTimeStamp)
    {
        _log.LogInformation("EmailTo: " + _settingsService.EmailTo);
        var targetEmails = _settingsService.EmailTo.Split(new char[]
        {
            ';', ','
        }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
        _log.LogInformation("EmailTo: " + _settingsService.EmailTo);
        var emailMessage = new SendGridMessage();

        var stateName = Convert.StatusToText(currentStatus);
        var message = $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp}\n\n";

        foreach (var hydroData in data)
            message += $"{hydroData.RiverName}: {hydroData.FlowLitersPerSecond} l/s v {hydroData.Timestamp}\n\n";

        emailMessage.Subject = $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp}";
        emailMessage.PlainTextContent = message;
        targetEmails.ForEach(te => emailMessage.AddTo(te));
        emailMessage.From = new EmailAddress(_settingsService.SendGridSenderIdentityEmail, _settingsService.SendGridSenderIdentityName);

        return emailMessage;
    }
}