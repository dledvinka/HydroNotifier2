namespace HydroNotifier.Core.Storage;

using HydroNotifier.Core.Utils;
using Microsoft.Azure.Cosmos.Table;

public class TableService : ITableService
{
    private const string _partitionKey = "Data";
    private const string _tableName = "HydroNotifier";
    private readonly ISettingsService _settingsService;
    private readonly CloudTable _table;

    public TableService(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        var storageAccount = CloudStorageAccount.Parse(_settingsService.TableStorageConnectionString);
        var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        _table = tableClient.GetTableReference(_tableName);
    }

    public List<FlowDataEntity> GetAll()
    {
        // Construct the query operation for all customer entities where PartitionKey="Smith".
        TableQuery<FlowDataEntity> query = new TableQuery<FlowDataEntity>()
            .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _partitionKey));

        return _table.ExecuteQuery(query).ToList();
    }

    public FlowDataEntity GetLastOrDefault()
    {
        // Construct the query operation for all customer entities where PartitionKey="Smith".
        TableQuery<FlowDataEntity> query = new TableQuery<FlowDataEntity>()
                                           .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _partitionKey))
                                           //.OrderByDesc("Timestamp") // works for Cosmos DB only
                                           .Take(1);

        var result = _table.ExecuteQuery(query).ToList();

        if (!result.Any())
            return null;

        return result[0];
    }

    public async Task<FlowDataEntity> InsertOrMergeAsync(FlowDataEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.RowKey))
        {
            var invertedTimeKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
            entity.RowKey = invertedTimeKey;
        }

        entity.PartitionKey = _partitionKey;

        var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
        var result = await _table.ExecuteAsync(insertOrMergeOperation);

        return result.Result as FlowDataEntity;
    }
}