using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNotifier.FunctionApp.Core
{
    public class HydroStatusCalculator
    {
        private const double NORMAL_TO_LOW_THRESHOLD = 630.0;
        private const double LOW_TO_NORMAL_THRESHOLD = 650.0;
        private const double NORMAL_TO_HIGH_THRESHOLD = 20000.0;
        private const double HIGH_TO_NORMAL_THRESHOLD = 13000.0;

        public HydroStatus GetCurrentStatus(List<HydroData> data, HydroStatus lastReportedStatus)
        {
            var flowSum = data.Sum(p => p.FlowLitersPerSecond);

            if (lastReportedStatus != HydroStatus.Low && flowSum <= NORMAL_TO_LOW_THRESHOLD)
                return HydroStatus.Low;

            if (lastReportedStatus != HydroStatus.High && flowSum >= NORMAL_TO_HIGH_THRESHOLD)
                return HydroStatus.High;

            if (lastReportedStatus != HydroStatus.Normal && flowSum > LOW_TO_NORMAL_THRESHOLD && flowSum < HIGH_TO_NORMAL_THRESHOLD)
                return HydroStatus.Normal;

            return lastReportedStatus;
        }
    }
}
