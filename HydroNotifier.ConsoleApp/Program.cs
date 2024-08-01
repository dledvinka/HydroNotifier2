namespace HydroNotifier.ConsoleApp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HydroNotifier.Core.Entities;
using HydroNotifier.Core.Storage;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        await TestWebScraper();
        //await TestTableAccess();
    }

    private static async Task TestWebScraper()
    {
        var hydroData = new List<HydroData>();
        var logMock = new Mock<ILogger>();

        using var httpClient = new HttpClient();
        hydroData.Add(await new WebScraper(HydroGuardQueries.LomnaQuery, httpClient, logMock.Object).GetLatestValuesAsync());
        hydroData.Add(await new WebScraper(HydroGuardQueries.OlseQuery, httpClient, logMock.Object).GetLatestValuesAsync());
    }

    private static async Task TestTableAccess()
    {
        // https://blog.bitscry.com/2017/05/30/appsettings-json-in-net-core-console-app/
        var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                            .AddJsonFile("appsettings.json", false)
                            .Build();

        var connectionString = configuration.GetValue<string>("STORAGE_CONNECTION_STRING");
        var tableName = "HydroNotifier";

        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        var table = tableClient.GetTableReference(tableName);

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
            OlseFlowLitersPerSecond = 50.0,
            LomnaFlowLitersPerSecond = 60.0,
            EmailNotificationSent = true,
            EmailNotificationJson = "TestEmailJson",
            SmsNotificationSent = true,
            SmsNotificationJson = "TestSmsJson",
            NexmoRemainingBalanceEur = "10.0",
            Status = HydroStatus.Normal.ToString(),
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
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.OlseFlowLitersPerSecond, entity.LomnaFlowLitersPerSecond);
        } while (token != null);
    }

    private static async Task MergeEntity(CloudTable table, FlowDataEntity entity)
    {
        var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

        var result = await table.ExecuteAsync(insertOrMergeOperation);
        FlowDataEntity inserted = result.Result as FlowDataEntity;
    }
}