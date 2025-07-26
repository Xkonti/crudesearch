using CrudeSearch.Api.Data.Interfaces;
using SurrealDb.Net;

namespace CrudeSearch.Api.Data;

public class ExampleRepository : IExampleRepository
{
    private readonly ISurrealDbClient _db;
    private readonly ILogger<ExampleRepository> _logger;

    public ExampleRepository(ISurrealDbClient db, ILogger<ExampleRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<string> GetExampleDataAsync()
    {
        try
        {
            _logger.LogInformation("Executing example query to test SurrealDB connection");
            
            var result = await _db.Query($"RETURN 'Hello, World!';");
            var message = result.GetValue<string>(0);
            
            _logger.LogInformation("Successfully retrieved example data: {Message}", message);
            
            return message ?? "No data returned";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute example query");
            throw;
        }
    }
}