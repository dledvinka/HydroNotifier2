namespace HydroNotifier.FunctionApp.Core
{
    public class HydroData
    {
        public HydroData(string riverName, string timestamp, double flowLitresPerSecond)
        {
            RiverName = riverName;
            Timestamp = timestamp;
            FlowLitresPerSecond = flowLitresPerSecond;
        }

        public string RiverName { get; }
        public string Timestamp { get; }
        public double FlowLitresPerSecond { get; }
    }
}