using backend.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace backend.Services;

public class VisitorService
{
    private readonly Container _container;

    public VisitorService(CosmosClient client, IConfiguration config)
    {
       
        var dbName = config["COSMOS_DB_DATABASE"] ?? "usersdb";
        var containerName = config["COSMOS_DB_CONTAINER"] ?? "users";
        var database = client.GetDatabase(dbName);
        _container = database.GetContainer(containerName);
    }

    public async Task<Visitor> AddVisitorAsync(string name)
    {
        var visitor = new Visitor
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Timestamp = DateTime.UtcNow
        };
             
        await _container.CreateItemAsync(visitor, new PartitionKey(visitor.Id));
        return visitor;
    }
}
