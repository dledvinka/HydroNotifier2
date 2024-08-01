namespace HydroNotifier.FunctionAppv4;

using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var cfgBuilder = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile($"appsettings.json", true, true)
                         .AddJsonFile($"appsettings.dev.json", true, true)
                         .AddEnvironmentVariables();
            
        var configuration = cfgBuilder.Build();
        builder.Services
               //.AddSingleton<ITester, Tester>()
               //.AddTimers()
               .AddLogging()
               .AddOptions();
    }
}