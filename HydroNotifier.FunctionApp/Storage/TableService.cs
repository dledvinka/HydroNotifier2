using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HydroNotifier.FunctionApp.Utils;
using Microsoft.Azure.Cosmos.Table;

namespace HydroNotifier.FunctionApp.Storage
{
    public class TableService : ITableService
    {
        private readonly ISettingsService _settingsService;
        private readonly CloudTable _table;

        public TableService(ISettingsService settingsService, string tableName)
        {
            _settingsService = settingsService;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_settingsService.TableStorageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            _table = tableClient.GetTableReference(tableName);
        }

        public async Task<T> InsertOrMergeAsync<T>(T entity) where T: class, ITableEntity
        {
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            TableResult result = await _table.ExecuteAsync(insertOrMergeOperation);
            
            return result as T;
        }

        public async Task<List<T>> GetAllAsync<T>() where T: class, ITableEntity, new()
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<T> query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Data"));
            return _table.ExecuteQuery<T>(query).ToList();
        }
    }

    //public class TableService : ITableService
    //{
    //    private readonly TableClient _tableClient;

    //    private TableService(TableClient tableClient)
    //    {
    //        _tableClient = tableClient;
    //    }
    //    public static TableService Create()
    //    {
    //        // https://docs.microsoft.com/en-us/dotnet/api/overview/azure/data.tables-readme-pre
    //        string storageUri = "https://hydronotifierfunctionapp.table.core.windows.net/HydroNotifier";
    //        string accountName = "hydronotifierfunctionapp";
    //        string storageAccountKey = "koIo7QnifLbQe8fLM2Z8Vb1rUvOj6kKiyIBuGqUuknViKav56O/hyhwVIcBs62DK9B0V3Y3g/GUuFBw+SOr9WQ==";
    //        string tableName = "HydroNotifier";

    //        var tableClient = new TableClient(
    //            new Uri(storageUri),
    //            tableName,
    //            new TableSharedKeyCredential(accountName, storageAccountKey));

    //        //tableClient.CreateIfNotExists();

    //        return new TableService(tableClient);
    //    }

    //    public void AddEntity<T>(T tableEntity) where T: class, ITableEntity, new()
    //    {
    //        _tableClient.AddEntity(tableEntity);
    //    }

    //    public List<T> GetAll<T>() where T : class, ITableEntity, new()
    //    {
    //        var dd = _tableClient.Query<FlowDataEntity>();

    //        foreach (var d in dd)
    //        {

    //        }

    //        var queryResult = _tableClient.Query<T>();
    //        var items = new List<T>();

    //        foreach (T item in queryResult)
    //        {
    //            items.Add(item);
    //        }

    //        return items;
    //    }
    //}
}
