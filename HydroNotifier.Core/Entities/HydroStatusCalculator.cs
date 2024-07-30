namespace HydroNotifier.Core.Entities;

using HydroNotifier.Core.Utils;

public class HydroStatusCalculator
{
    private const double HIGH_TO_NORMAL_THRESHOLD = 13000.0;
    private const double LOW_TO_NORMAL_THRESHOLD = 650.0;
    private const double NORMAL_TO_HIGH_THRESHOLD = 20000.0;
    private const double NORMAL_TO_LOW_THRESHOLD = 630.0;
    private readonly ITelemetry _tc;

    public HydroStatusCalculator(ITelemetry tc) => _tc = tc;

    public HydroStatus GetCurrentStatus(List<HydroData> data, HydroStatus lastReportedStatus)
    {
        var flowSum = data.Sum(p => p.FlowLitersPerSecond);
        var status = lastReportedStatus;

        if (lastReportedStatus != HydroStatus.Low && flowSum <= NORMAL_TO_LOW_THRESHOLD)
            status = HydroStatus.Low;
        else if (lastReportedStatus != HydroStatus.High && flowSum >= NORMAL_TO_HIGH_THRESHOLD)
            status = HydroStatus.High;
        else if (lastReportedStatus != HydroStatus.Normal &&
                 flowSum > LOW_TO_NORMAL_THRESHOLD && flowSum < HIGH_TO_NORMAL_THRESHOLD)
            status = HydroStatus.Normal;

        ReportTelemetry(status, flowSum);

        return status;
    }

    private void ReportTelemetry(HydroStatus status, double flowSum)
    {
        _tc.TrackMetric("FlowSum", flowSum);
        _tc.TrackMetric("HydroStatus", (double) status);
    }
}