namespace FunctionApp2
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