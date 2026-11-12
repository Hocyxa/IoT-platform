using IoT.ControlPlane.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.Fixture;

public sealed class DatabaseFixture : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _postgres;

    public DatabaseFixture(PostgreSqlFixture postgres)
    {
        _postgres = postgres;
    }

    public async Task<ControlPlaneDbContext> CreateDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<ControlPlaneDbContext>()
            .UseNpgsql(_postgres.ConnectionString)
            .Options;

        var dbContext = new ControlPlaneDbContext(options);

        await dbContext.Database.MigrateAsync();

        return dbContext;
    }
}
