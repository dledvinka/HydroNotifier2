using HydroNotifier.FunctionApp.Core;
using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Convert = HydroNotifier.FunctionApp.Utils.Convert;

namespace HydroNotifier.FunctionApp.Notifications
{
    public class SmsMessageBuilder
    {
        public SMS.SMSRequest BuildMessage(List<HydroData> data, HydroStatus currentStatus, DateTime stateChangedTimeStamp, string smsTo)
        {
            var request = new SMS.SMSRequest();

            string stateName = Convert.StatusToText(currentStatus);
            decimal flowSum = data.Sum(p => p.FlowLitersPerSecond);

            string message = $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp.ToString(CultureInfo.CreateSpecificCulture("cs-CZ"))}, Prutok: {flowSum.ToString(CultureInfo.CreateSpecificCulture("cs-CZ"))} l/s";

            request.from = "HydroNotifier";
            request.to = smsTo;
            request.text = message;

            return request;
        }
    }
}
