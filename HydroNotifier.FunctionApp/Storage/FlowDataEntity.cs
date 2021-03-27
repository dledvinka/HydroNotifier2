using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.FunctionApp.Storage
{
    public class FlowDataEntity : ITableEntity
    {
        public string RiverName { get; set; }
        public decimal FlowLitersPerSecond { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
