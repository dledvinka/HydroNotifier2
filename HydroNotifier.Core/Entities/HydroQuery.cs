namespace HydroNotifier.Core.Entities;

public class HydroQuery
{
    public string Name { get; }

    public string Url { get; }

    public HydroQuery(string name, string url)
    {
        Name = name;
        Url = url;
    }
}