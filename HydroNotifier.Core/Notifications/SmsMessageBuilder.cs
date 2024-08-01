namespace HydroNotifier.Core.Notifications;

using System.Globalization;
using HydroNotifier.Core.Entities;
using Nexmo.Api;
using Vonage.Messaging;
using Convert = HydroNotifier.Core.Utils.Convert;

public class SmsMessageBuilder
{
    public SMS.SMSRequest BuildMessage(List<HydroData> data, HydroStatus currentStatus, DateTime stateChangedTimeStamp, string smsTo)
    {
        var request = new SMS.SMSRequest();

        var stateName = Convert.StatusToText(currentStatus);
        var flowSum = data.Sum(p => p.FlowLitersPerSecond);

        var message =
            $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp.ToString(CultureInfo.CreateSpecificCulture("cs-CZ"))}, Prutok: {flowSum.ToString(CultureInfo.CreateSpecificCulture("cs-CZ"))} l/s";

        request.from = "HydroNotifier";
        request.to = smsTo;
        request.text = message;

        return request;
    }

    public SendSmsRequest BuildMessage2(List<HydroData> data, HydroStatus currentStatus, DateTime stateChangedTimeStamp, string smsTo)
    {
        var stateName = Convert.StatusToText(currentStatus);
        var flowSum = data.Sum(p => p.FlowLitersPerSecond);

        var message =
            $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp.ToString(CultureInfo.CreateSpecificCulture("cs-CZ"))}, Prutok: {flowSum.ToString(CultureInfo.CreateSpecificCulture("cs-CZ"))} l/s";

        var request = new SendSmsRequest()
        {
            From = "HydroNotifier",
            To = smsTo,
            Text = message
        };

        return request;
    }
}