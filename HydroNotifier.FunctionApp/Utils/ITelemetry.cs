using System.Collections.Generic;

namespace HydroNotifier.FunctionApp.Utils
{
    public interface ITelemetry
    {
        void TrackMetric(string name, decimal value);
        void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
    }
}
