using Microsoft.EntityFrameworkCore;
using ShadowGaze.Core.Services.Containerization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ShadowGaze.Data.Services;

var host = Host.CreateDefaultBuilder()
    .ConfigureHostConfiguration(builder => builder.AddJsonFile("secret.json"))
    .ConfigureLogging(builder => builder
        .ClearProviders()
        .AddNLog())
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<DatabaseContext>(opt =>
        {
            opt.UseNpgsql(context.Configuration.GetConnectionString("Default"));
            opt.UseSnakeCaseNamingConvention();
        });

        services.AddPublicBot();
    })
    .Build();
await host.RunAsync();