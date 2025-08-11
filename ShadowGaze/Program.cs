using ShadowGaze.Core.Services.Containerization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

var host = Host.CreateDefaultBuilder()
    .ConfigureHostConfiguration(builder => builder.AddJsonFile("secret.json"))
    .ConfigureLogging(builder => builder
        .ClearProviders()
        .AddNLog())
    .ConfigureServices(services => services.AddPublicBot())
    .Build();
await host.RunAsync();