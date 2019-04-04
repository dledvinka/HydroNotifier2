using System;
using System.Collections.Generic;
using System.Text;

namespace HydroNotifier.FunctionApp.Utils
{
    public interface ITelemetry
    {
        void TrackMetric(string name, double value, IDictionary<string, string> properties = null);
    }
}
