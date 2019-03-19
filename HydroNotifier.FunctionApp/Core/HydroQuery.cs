namespace HydroNotifier.FunctionApp.Core
{
    public class HydroQuery
    {
        public HydroQuery(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string Url { get; }
        public string Name { get; }
    }
}