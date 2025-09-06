using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace RegistationApplication;

public class Registeruser
{
    private readonly ILogger<Registeruser> _logger;

    public Registeruser(ILogger<Registeruser> logger)
    {
        _logger = logger;
    }

    [Function("Registeruser")]
    public void Run([CosmosDBTrigger(
        databaseName: "usersdb",
        containerName: "users",
        Connection = "COSMOS_CONN",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)]
        IReadOnlyList<MyDocument> input)
    {
        if (input != null && input.Count > 0)
        {
            _logger.LogInformation("Documents modified: {count}", input.Count);
            _logger.LogInformation("First document Id: {id}", input[0].id);
        }
    }
}

public class MyDocument
{
    public string id { get; set; }
    public string name { get; set; }
    public DateTime timestamp { get; set; }
}
