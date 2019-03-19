using System;
using SendGrid.Helpers.Mail;

namespace HydroNotifier.Core
{
    internal class EmailMessageBuilder
    {
        private readonly ISettingsService _settingsService;

        public EmailMessageBuilder(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public SendGrid.Helpers.Mail.SendGridMessage BuildMessage(HydroData lomnaData, HydroData olseData, HydroStatus currentStatus, in DateTime stateChangedTimeStamp)
        {
            var emailMessage = new SendGrid.Helpers.Mail.SendGridMessage();

            string stateName = Convert.StatusToText(currentStatus);
            string message =
                $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp}\n\n" +
                $"{lomnaData.RiverName}: {lomnaData.FlowLitresPerSecond} l/s v {lomnaData.Timestamp}\n" +
                $"{olseData.RiverName}: {olseData.FlowLitresPerSecond} l/s v {olseData.Timestamp}";

            emailMessage.Subject = $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp}";
            emailMessage.PlainTextContent = message;
            emailMessage.AddTo("ledvinka.david@gmail.com");
            emailMessage.AddTo("david.ledvinka@post.cz");
            emailMessage.From = new EmailAddress("hydronotifier@no-reply.com", "HydroNotifier");

            return emailMessage;
        }
    }
}