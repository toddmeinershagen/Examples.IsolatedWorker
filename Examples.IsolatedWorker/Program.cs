using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Paycor.FeatureFlags;
using Paycor.FeatureFlags.AzureAppConfiguration;

var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<IFlagService, FlagService>();
        services.AddFeatureManagement();
    });

hostBuilder.ConfigureAppConfiguration((context, config) =>
{
    //Note - these settings are up to the team.
    var optional = false;
    var reloadOnChange = true;
    var cacheExpiration = TimeSpan.FromSeconds(5);

    config.AddAzureAppConfiguration(options =>
    {
        options
            .Connect("")
            .Select(KeyFilter.Any, "qa")
            .UseFeatureFlags(f => f.CacheExpirationTime = cacheExpiration);

        //note:     If you want to refresh configuration automatically, you will want to assign the ambient Refresher.
        //          This removes the need to differentiate your set up for asp.net mvc/core which use middleware compared to azure functions.
        //          If you do not set this value, there is a default no-op refresher that will be used instead that will not interfere
        //          with other set up you have for asp.net mvc/core or azure functions.
        Refresher.ProvidedBy(options.GetRefresher);
    });
});
    
var host = hostBuilder.Build();
host.Run();
