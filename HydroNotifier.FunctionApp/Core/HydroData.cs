namespace HydroNotifier.FunctionApp.Core
{
    public class HydroData
    {
        public HydroData(string riverName, string timestamp, double flowLitersPerSecond)
        {
            RiverName = riverName;
            Timestamp = timestamp;
            FlowLitersPerSecond = flowLitersPerSecond;
        }

        public string RiverName { get; }
        public string Timestamp { get; }
        public double FlowLitersPerSecond { get; }
    }
}