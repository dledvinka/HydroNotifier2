﻿namespace HydroNotifier.Core.Utils;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

public class ApplicationInsightsTelemetry : ITelemetry
{
    private readonly TelemetryClient _tc = new TelemetryClient(TelemetryConfiguration.CreateDefault());

    public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
    {
        _tc.TrackEvent(eventName, properties, metrics);
    }

    public void TrackMetric(string name, double value)
    {
        _tc.TrackMetric(name, value);
    }
}