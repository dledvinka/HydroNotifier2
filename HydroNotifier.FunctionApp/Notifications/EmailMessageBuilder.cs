using System;
using System.Collections.Generic;
using HydroNotifier.FunctionApp.Core;
using HydroNotifier.FunctionApp.Utils;
using SendGrid.Helpers.Mail;
using Convert = HydroNotifier.FunctionApp.Utils.Convert;

namespace HydroNotifier.FunctionApp.Notifications
{
    internal class EmailMessageBuilder
    {
        private readonly ISettingsService _settingsService;

        public EmailMessageBuilder(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public SendGrid.Helpers.Mail.SendGridMessage BuildMessage(List<HydroData> data, HydroStatus currentStatus, in DateTime stateChangedTimeStamp)
        {
            var emailMessage = new SendGrid.Helpers.Mail.SendGridMessage();

            string stateName = Convert.StatusToText(currentStatus);
            string message = $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp}\n\n";

            foreach (var hydroData in data)
            {
                message += $"{hydroData.RiverName}: {hydroData.FlowLitresPerSecond} l/s v {hydroData.Timestamp}\n\n";
            }

            emailMessage.Subject = $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp}";
            emailMessage.PlainTextContent = message;
            emailMessage.AddTo("ledvinka.david@gmail.com");
            emailMessage.AddTo("david.ledvinka@post.cz");
            emailMessage.From = new EmailAddress("hydronotifier@no-reply.com", "HydroNotifier");

            return emailMessage;
        }
    }
}