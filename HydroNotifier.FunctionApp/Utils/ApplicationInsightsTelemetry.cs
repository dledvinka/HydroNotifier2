using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace HydroNotifier.FunctionApp.Utils
{
    public class ApplicationInsightsTelemetry : ITelemetry
    {
        private readonly TelemetryClient _tc = new TelemetryClient();

        public void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
        {
            _tc.TrackMetric(name, value, properties);
        }
    }
}