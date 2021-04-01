using System.Collections.Generic;
using System.Threading.Tasks;

namespace HydroNotifier.FunctionApp.Storage
{
    public interface ITableService
    {
        List<FlowDataEntity> GetAll();
        FlowDataEntity GetLastOrDefault();
        Task<FlowDataEntity> InsertOrMergeAsync(FlowDataEntity entity);
    }
}
