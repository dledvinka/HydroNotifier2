using System;
using System.Collections.Generic;
using System.Linq;
using HydroNotifier.FunctionApp.Core;
using Nexmo.Api;
using Convert = HydroNotifier.FunctionApp.Utils.Convert;

namespace HydroNotifier.FunctionApp.Notifications
{
    public class SmsMessageBuilder
    {
        public SMS.SMSRequest BuildMessage(List<HydroData> data, HydroStatus currentStatus, DateTime stateChangedTimeStamp, string smsTo)
        {
            var request = new SMS.SMSRequest();

            string stateName = Convert.StatusToText(currentStatus);
            double flowSum = data.Sum(p => p.FlowLitresPerSecond);

            string message = $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp}, Prutok: {flowSum} l/s";

            request.from = "HydroNotifier";
            request.to = smsTo;
            request.text = message;

            return request;
        }
    }
}
