using HydroNotifier.FunctionApp.Core;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.FunctionApp.Storage
{
    public class FlowDataEntity : TableEntity
    {
        public string Status { get; set; }
        public double OlseFlowLitersPerSecond { get; set; }
        public double LomnaFlowLitersPerSecond { get; set; }
        public bool EmailNotificationSent { get; set; }
        public string EmailNotificationJson { get; set; }
        public bool SmsNotificationSent { get; set; }
        public string SmsNotificationJson { get; set; }
        public string NexmoRemainingBalanceEur { get; set; }
    }
}
