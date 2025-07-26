using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace CrudeSearch.IntegrationTests.Infrastructure;

public class SurrealDbContainerFixture : IAsyncLifetime
{
    private readonly IContainer _container;

    public SurrealDbContainerFixture()
    {
        Port = GetAvailablePort();
        
        _container = new ContainerBuilder()
            .WithImage("surrealdb/surrealdb:v2.3.7")
            .WithCommand("start", "--log", "info", "--user", "root", "--pass", "root", "memory")
            .WithPortBinding(Port, 8000)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("Started web server on"))
            .Build();
    }

    public string ConnectionString => $"Endpoint=ws://localhost:{Port}/rpc;NS=test;DB=test;User=root;Pass=root";
    
    public int Port { get; }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        
        // Give SurrealDB a moment to fully initialize after log message appears
        await Task.Delay(1000);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private static int GetAvailablePort()
    {
        using var socket = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        socket.Start();
        var port = ((System.Net.IPEndPoint)socket.LocalEndpoint).Port;
        socket.Stop();
        return port;
    }

    public static SurrealDbContainerFixture CreateWithPort(int port)
    {
        throw new NotSupportedException("Custom ports not supported with xUnit class fixtures. Use collection fixtures instead.");
    }
}