using System;
using System.Collections.Generic;
using System.Linq;
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

        public SendGridMessage BuildMessage(List<HydroData> data, HydroStatus currentStatus, DateTime stateChangedTimeStamp)
        {
            var targetEmails = _settingsService.EmailTo.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
            var emailMessage = new SendGridMessage();

            string stateName = Convert.StatusToText(currentStatus);
            string message = $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp}\n\n";

            foreach (var hydroData in data)
            {
                message += $"{hydroData.RiverName}: {hydroData.FlowLitersPerSecond} l/s v {hydroData.Timestamp}\n\n";
            }

            emailMessage.Subject = $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp}";
            emailMessage.PlainTextContent = message;
            targetEmails.ForEach(te => emailMessage.AddTo(te));
            emailMessage.From = new EmailAddress("hydronotifier@no-reply.com", "HydroNotifier");

            return emailMessage;
        }
    }
}