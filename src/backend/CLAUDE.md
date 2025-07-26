# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Crude Search is an open-source full-text and semantic search solution. This ASP.NET Core 9.0 backend provides a RESTful API with dual search modes (full-text and semantic/vector search) and two-tier caching (in-memory + Redis).

## Solution Architecture

The solution uses a clean architecture pattern with three main projects:

- **CrudeSearch.Api**: Main Web API project with controllers, services, and infrastructure
- **CrudeSearch.UnitTests**: Isolated unit tests with mocking
- **CrudeSearch.IntegrationTests**: Full-stack tests using Testcontainers

### Key Technologies
- **.NET 9.0** with nullable reference types and implicit usings
- **SurrealDB**: Multi-model database with SurrealQL (SQL-like syntax) for data persistence
- **Redis**: Distributed caching layer
- **xUnit + FluentAssertions + NSubstitute + Bogus**: Testing stack
- **Testcontainers**: Container-based integration testing

## Development Commands

### Building and Running
```bash
# Build entire solution
dotnet build

# Run API (listens on http://localhost:5109, https://localhost:7175)
dotnet run --project CrudeSearch.Api

# Development with hot reload
dotnet watch --project CrudeSearch.Api
```

### Testing
```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test CrudeSearch.UnitTests/

# Run only integration tests (requires Docker)
dotnet test CrudeSearch.IntegrationTests/

# Run with coverage collection
dotnet test --collect:"XPlat Code Coverage"

# List available tests
dotnet test --list-tests

# Run specific test
dotnet test --filter "FullyQualifiedName=CrudeSearch.UnitTests.UnitTest1.Test1"
```

## Project Structure and Layering

The API project follows a layered architecture:

```
CrudeSearch.Api/
├── Controllers/           # API endpoints
├── Services/             # Business logic
│   └── Interfaces/       # Service contracts  
├── Data/                 # Data access layer
│   └── Interfaces/       # Repository abstractions
├── Models/               # Data transfer objects
│   ├── Domain/           # Core entities
│   ├── Requests/         # API request DTOs
│   └── Responses/        # API response DTOs
├── Infrastructure/       # Cross-cutting concerns
│   ├── Authentication/   # Authentication handlers
│   ├── Extensions/       # DI container extensions
│   ├── Filters/          # Action filters
│   └── Middleware/       # Custom middleware
├── Configuration/        # Strongly-typed configuration
└── Background/           # Background services
```

## Database Integration (SurrealDB)

SurrealDB integration uses the official .NET SDK with dependency injection:

```csharp
// Program.cs setup
services.AddSurreal(configuration.GetConnectionString("SurrealDB"));

// Usage in services
public class SearchService
{
    private readonly ISurrealDbClient _surrealDb;
    
    public SearchService(ISurrealDbClient surrealDb)
    {
        _surrealDb = surrealDb;
    }
}
```

Connection string format:
```json
{
  "ConnectionStrings": {
    "SurrealDB": "Server=http://127.0.0.1:8000;Namespace=crudesearch;Database=main;Username=root;Password=root"
  }
}
```

## Caching Architecture

Two-tier caching strategy:
1. **In-memory cache**: Fast, size-limited cache for hot data
2. **Redis distributed cache**: Persistent cache for search results

```csharp
// Setup pattern
services.AddMemoryCache(options => {
    options.SizeLimit = configuration.GetValue<int>("Cache:InMemory:SizeLimit");
});

services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"))
);

services.AddStackExchangeRedisCache(options => {
    options.Configuration = configuration.GetConnectionString("Redis");
});
```

## Authentication Model

- **Single management user**: Administrative operations
- **Public tokens**: Simple token-based access for user frontends  
- **Backend integration**: Direct API access for data operations

## Search Implementation

### Dual Search Modes
1. **Full-text search**: Traditional keyword-based search with relevance scoring
2. **Semantic search**: Vector-based search using embeddings and external AI models

### Client Integration Points
- **Management frontend**: Separate administrative interface
- **User frontends**: Public search interfaces using token auth
- **User backends**: Direct API integration for data insertion/updates

## Testing Strategy

### Unit Tests
- Use **NSubstitute** for mocking dependencies
- Use **Bogus** for generating realistic test data
- Use **FluentAssertions** for readable test assertions
- Focus on isolated component testing

### Integration Tests  
- Use **Microsoft.AspNetCore.Mvc.Testing** for full API testing
- Use **Testcontainers** (Redis container already configured) for real dependencies
- Test full request/response cycles with real database interactions

## Code Standards

- **Nullable reference types**: Enabled across all projects
- **Implicit usings**: Enabled for cleaner code
- **Async/await**: Use for all I/O operations
- **Dependency injection**: Use constructor injection pattern
- **Configuration**: Use strongly-typed configuration classes