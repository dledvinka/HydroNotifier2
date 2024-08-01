namespace HydroNotifier.Core.Storage;

public interface ITableService
{
    List<FlowDataEntity> GetAll();
    FlowDataEntity GetLastOrDefault();
    Task<FlowDataEntity> InsertOrMergeAsync(FlowDataEntity entity);
}