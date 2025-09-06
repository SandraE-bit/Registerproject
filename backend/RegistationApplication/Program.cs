using backend.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(
        (context, services) =>
        {
            var cosmosClient = new CosmosClient(
                Environment.GetEnvironmentVariable("CosmosDbConnectionString")
            );
            services.AddSingleton(cosmosClient);
            services.AddSingleton<VisitorService>();
        }
    )
    .Build();

host.Run();