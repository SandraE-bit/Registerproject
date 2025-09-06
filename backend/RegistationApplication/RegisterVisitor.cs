using backend.Models;
using backend.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace RegistationApplication;

public class RegisterVisitor
{
    private readonly VisitorService _visitorService;
    private readonly ILogger<RegisterVisitor> _logger;

    public RegisterVisitor(VisitorService visitorService, ILoggerFactory loggerFactory)
    {
        _visitorService = visitorService;
        _logger = loggerFactory.CreateLogger<RegisterVisitor>();
    }

    [Function("RegisterVisitor")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(body))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Missing name in request body.");
            return bad;
        }
                
        string name;
        try
        {
       
            var doc = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
            if (doc != null && doc.TryGetValue("name", out var n) && !string.IsNullOrWhiteSpace(n))
            {
                name = n.Trim();
            }
            else
            {
               
                name = body.Trim();
            }
        }
        catch
        {
                name = body.Trim();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Name empty after parsing.");
            return bad;
        }

        _logger.LogInformation("Registering visitor: {name}", name);

        Visitor visitor;
        try
        {
            visitor = await _visitorService.AddVisitorAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save visitor");
            var err = req.CreateResponse(HttpStatusCode.InternalServerError);
            await err.WriteStringAsync("Failed to save visitor.");
            return err;
        }

        var ok = req.CreateResponse(HttpStatusCode.OK);
        await ok.WriteAsJsonAsync(visitor);
        return ok;
    }
}

