using Testcontainers.PostgreSql;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.Fixture;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder("postgres:17-alpine")
            .WithDatabase("iot_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
