using BigBroPublicBot.Core.Services.Containerization;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .AddPublicBot()
    .Build();

await host.RunAsync();