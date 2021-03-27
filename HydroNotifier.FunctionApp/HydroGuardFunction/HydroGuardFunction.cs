using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HydroNotifier.FunctionApp.Core;
using HydroNotifier.FunctionApp.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SendGrid.Helpers.Mail;

namespace HydroNotifier.FunctionApp
{
    public static class HydroGuardFunction
    {
        [FunctionName("HydroGuardTimerFunction")]
        public static async void Run(
            [TimerTrigger("*/10 * * * * *")]TimerInfo myTimer,
            //[Table("HydroNotifierFlowData", Connection = "DefaultEndpointsProtocol=https;AccountName=hydronotifierfunctionapp;AccountKey=b6/bnGXZI3Gh1Jh5ScaKNboMAZCYbDwv/PYJpGemcOf31JlzjhgZOlVfqkJ2VioOIhKCNk5C8fZYZsnUMrxE9w==;EndpointSuffix=core.windows.net")]
            //[TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [SendGrid] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log,
            ExecutionContext executionContext)
        {
            try
            {
                // https://www.rickvandenbosch.net/blog/using-table-storage-bindings-in-azure-functions/

                var connectionString = "DefaultEndpointsProtocol=https;AccountName=hydronotifierfunctionapp;AccountKey=b6/bnGXZI3Gh1Jh5ScaKNboMAZCYbDwv/PYJpGemcOf31JlzjhgZOlVfqkJ2VioOIhKCNk5C8fZYZsnUMrxE9w==;EndpointSuffix=core.windows.net";
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("HydroNotifierFlowData");
                var exists = await table.CreateIfNotExistsAsync();
                var uri = table.StorageUri;

                //var result = await GetEntitiesFromTable(table);

                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                var stateService = new StateService(executionContext.FunctionDirectory);
                var settingsService = new SettingsService();
                var telemetry = new ApplicationInsightsTelemetry();

                var hg = new HydroGuard(messageCollector, stateService, settingsService, log, telemetry);
                await hg.DoAsync();
                await messageCollector.FlushAsync();
                log.LogInformation("Done.");
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, $"Exception found {ex.Message}");
            }
        }

        private static async Task<IEnumerable<T>> GetEntitiesFromTable<T>(CloudTable table) where T : ITableEntity, new()
        {
            TableQuerySegment<T> querySegment = null;
            var entities = new List<T>();
            var query = new TableQuery<T>();

            do
            {
                querySegment = await table.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken);
                entities.AddRange(querySegment.Results);
            } while (querySegment.ContinuationToken != null);

            return entities;
        }
    }
}
