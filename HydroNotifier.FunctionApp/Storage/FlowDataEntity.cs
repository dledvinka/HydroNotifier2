using Azure;
using Azure.Data.Tables;
using HydroNotifier.FunctionApp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.FunctionApp.Storage
{
    public class FlowDataEntity : ITableEntity
    {
        public HydroStatus Status { get; set; }
        public decimal OlseFlowLitersPerSecond { get; set; }
        public decimal LomnaFlowLitersPerSecond { get; set; }
        public bool EmailNotificationSent { get; set; }
        public string EmailNotificationJson { get; set; }
        public bool SmsNotificationSent { get; set; }
        public string SmsNotificationText { get; set; }
        public decimal NexmoCreditRemaining { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
