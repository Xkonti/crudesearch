using CrudeSearch.Api.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CrudeSearch.UnitTests;

public class ExampleRepositoryTests
{
    [Fact]
    public void ExampleRepository_Constructor_ShouldNotThrow()
    {
        // Arrange
        var mockClient = Substitute.For<SurrealDb.Net.ISurrealDbClient>();
        var mockLogger = Substitute.For<ILogger<ExampleRepository>>();

        // Act
        var act = () => new ExampleRepository(mockClient, mockLogger);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ExampleRepository_Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var mockClient = Substitute.For<SurrealDb.Net.ISurrealDbClient>();
        var mockLogger = Substitute.For<ILogger<ExampleRepository>>();

        // Act
        var repository = new ExampleRepository(mockClient, mockLogger);

        // Assert
        repository.Should().NotBeNull();
    }
}