using backend.Services;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Cosmos;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;
        var conn = config["CosmosDbConnectionString"] ?? config["COSMOS_CONN:ConnectionString"] ?? Environment.GetEnvironmentVariable("CosmosDbConnectionString");
        if (string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException("Missing CosmosDbConnectionString in configuration.");

        var client = new CosmosClient(conn);
        services.AddSingleton(client);
        services.AddSingleton<VisitorService>();
    })
    .Build();

host.Run();
