using HydroNotifier.FunctionApp.Storage;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HydroNotifier.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // https://blog.bitscry.com/2017/05/30/appsettings-json-in-net-core-console-app/
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            string connectionString = configuration.GetValue<string>("STORAGE_CONNECTION_STRING");
            string tableName = "HydroNotifier";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);

            //bool created = table.CreateIfNotExists();

            var flowData = CreateFlowData();
            await MergeEntity(table, flowData);

            await GetAllEntitiesAsync(table);


        }

        private static FlowDataEntity CreateFlowData()
        {
            var flowData = new FlowDataEntity()
            {
                PartitionKey = "Data",
                RowKey = Guid.NewGuid().ToString(),
                OlseFlowLitersPerSecond = 50.0,
                LomnaFlowLitersPerSecond = 60.0,
                EmailNotificationSent = true,
                EmailNotificationJson = "TestEmailJson",
                SmsNotificationSent = true,
                SmsNotificationJson = "TestSmsJson",
                NexmoRemainingBalanceEur = "10.0",
                Status = FunctionApp.Core.HydroStatus.Normal.ToString(),
                Timestamp = DateTime.UtcNow
            };

            return flowData;
        }

        //private static async Task Query(CloudTable table)
        //{
        //    TableOperation retrieveOperation = TableOperation.
        //}

        private static async Task GetAllEntitiesAsync(CloudTable stockDataTable)
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<FlowDataEntity> query = new TableQuery<FlowDataEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Data"));

            // Print the fields for each customer.
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<FlowDataEntity> resultSegment = await stockDataTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (FlowDataEntity entity in resultSegment.Results)
                {
                    Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.OlseFlowLitersPerSecond, entity.LomnaFlowLitersPerSecond);
                }
            } while (token != null);
        }

        private static async Task MergeEntity(CloudTable table, FlowDataEntity entity)
        {
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
            FlowDataEntity inserted = result.Result as FlowDataEntity;
        }
    }
}
