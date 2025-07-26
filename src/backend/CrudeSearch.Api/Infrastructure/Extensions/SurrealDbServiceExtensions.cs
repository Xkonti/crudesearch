using CrudeSearch.Api.Data;
using CrudeSearch.Api.Data.Interfaces;

namespace CrudeSearch.Api.Infrastructure.Extensions;

public static class SurrealDbServiceExtensions
{
    public static IServiceCollection AddSurrealDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SurrealDB");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("SurrealDB connection string is required but not found in configuration.");
        }

        services.AddSurreal(connectionString);

        services.AddScoped<IExampleRepository, ExampleRepository>();

        return services;
    }
}