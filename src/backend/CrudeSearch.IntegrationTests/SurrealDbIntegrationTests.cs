using CrudeSearch.Api.Data;
using CrudeSearch.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SurrealDb.Net;

namespace CrudeSearch.IntegrationTests;

public class SurrealDbIntegrationTests : IClassFixture<SurrealDbContainerFixture>
{
    private readonly SurrealDbContainerFixture _containerFixture;
    private readonly IServiceProvider _serviceProvider;

    public SurrealDbIntegrationTests(SurrealDbContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
        
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task ExampleRepository_GetExampleDataAsync_ShouldReturnHelloWorld()
    {
        // Arrange
        var endpoint = $"ws://localhost:{_containerFixture.Port}/rpc";
        await using var surrealDbClient = new SurrealDbClient(endpoint);
        
        // Authenticate and select database
        await surrealDbClient.SignIn(new SurrealDb.Net.Models.Auth.RootAuth
        {
            Username = "root",
            Password = "root"
        });
        await surrealDbClient.Use("test", "test");
        
        var logger = _serviceProvider.GetRequiredService<ILogger<ExampleRepository>>();
        var repository = new ExampleRepository(surrealDbClient, logger);

        // Act
        var result = await repository.GetExampleDataAsync();

        // Assert
        result.Should().Be("Hello, World!");
    }

    [Fact]
    public async Task SurrealDbContainer_ShouldBeHealthy()
    {
        // Arrange
        var endpoint = $"ws://localhost:{_containerFixture.Port}/rpc";
        await using var surrealDbClient = new SurrealDbClient(endpoint);

        // Act & Assert - if Health() throws, the test will fail
        await surrealDbClient.Health();
    }
}