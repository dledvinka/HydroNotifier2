using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.Core
{
    public class SmsMessageBuilder
    {
        public SMS.SMSRequest BuildMessage(HydroData data1, HydroData data2, HydroStatus currentStatus, DateTime stateChangedTimeStamp, string smsTo)
        {
            var request = new SMS.SMSRequest();

            string stateName = Convert.StatusToText(currentStatus);

            string message = $"Jablunkov MVE, Stav: {stateName}, Datum: {stateChangedTimeStamp}, Prutok: {data1.FlowLitresPerSecond + data2.FlowLitresPerSecond} l/s";

            request.from = "HydroNotifier";
            request.to = smsTo;
            request.text = message;

            return request;
        }
    }
}
