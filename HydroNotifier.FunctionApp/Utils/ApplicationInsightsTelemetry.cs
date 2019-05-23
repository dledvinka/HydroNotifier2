﻿using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace HydroNotifier.FunctionApp.Utils
{
    public class ApplicationInsightsTelemetry : ITelemetry
    {
        private readonly TelemetryClient _tc = new TelemetryClient();

        public void TrackMetric(string name, double value)
        {
            _tc.TrackMetric(name, value);
        }

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            _tc.TrackEvent(eventName, properties, metrics);
        }
    }
}