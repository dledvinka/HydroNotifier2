using Azure.Data.Tables;
using HydroNotifier.FunctionApp.Storage;
using System;

namespace HydroNotifier.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var tableService = TableService.Create();

            // Create the table in the service.
            //tableClient.CreateIfNotExists();

            //https://github.com/Azure/azure-sdk-for-net/blob/Azure.Data.Tables_12.0.0-beta.6/sdk/tables/Azure.Data.Tables/samples/Sample2CreateDeleteEntities.md
            var flowData = new FlowDataEntity()
            {
                PartitionKey = "DATA",
                RowKey = Guid.NewGuid().ToString(),
                OlseFlowLitersPerSecond = 50.0M,
                LomnaFlowLitersPerSecond = 60.0M,
                EmailNotificationSent = true,
                EmailNotificationJson = "TestEmailJson",
                SmsNotificationSent = true,
                SmsNotificationJson = "TestSmsJson",
                NexmoRemainingBalanceEur = "10.0",
                Status = FunctionApp.Core.HydroStatus.Normal,
                Timestamp = DateTime.UtcNow
            };

            tableService.AddEntity(flowData);

            var tt = tableService.GetAll<FlowDataEntity>();
        }
    }
}
