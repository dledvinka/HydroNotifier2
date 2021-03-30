using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azure.Data.Tables;

namespace HydroNotifier.FunctionApp.Storage
{
    public class TableService : ITableService
    {
        private readonly TableClient _tableClient;

        private TableService(TableClient tableClient)
        {
            _tableClient = tableClient;
        }
        public static TableService Create()
        {
            // https://docs.microsoft.com/en-us/dotnet/api/overview/azure/data.tables-readme-pre
            string storageUri = "https://hydronotifierfunctionapp.table.core.windows.net/HydroNotifier";
            string accountName = "hydronotifierfunctionapp";
            string storageAccountKey = "koIo7QnifLbQe8fLM2Z8Vb1rUvOj6kKiyIBuGqUuknViKav56O/hyhwVIcBs62DK9B0V3Y3g/GUuFBw+SOr9WQ==";
            string tableName = "HydroNotifier";

            var tableClient = new TableClient(
                new Uri(storageUri),
                tableName,
                new TableSharedKeyCredential(accountName, storageAccountKey));

            //tableClient.CreateIfNotExists();

            return new TableService(tableClient);
        }

        public void AddEntity<T>(T tableEntity) where T: class, ITableEntity, new()
        {
            _tableClient.AddEntity(tableEntity);
        }

        public List<T> GetAll<T>() where T : class, ITableEntity, new()
        {
            var dd = _tableClient.Query<FlowDataEntity>();

            foreach (var d in dd)
            {

            }
            
            var queryResult = _tableClient.Query<T>();
            var items = new List<T>();

            foreach (T item in queryResult)
            {
                items.Add(item);
            }

            return items;
        }
    }
}
