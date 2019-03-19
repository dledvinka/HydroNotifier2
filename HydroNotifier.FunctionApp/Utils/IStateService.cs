using HydroNotifier.FunctionApp.Core;

namespace HydroNotifier.FunctionApp.Utils
{
    public interface IStateService
    {
        HydroStatus GetStatus();
        void SetStatus(HydroStatus status);
    }
}
