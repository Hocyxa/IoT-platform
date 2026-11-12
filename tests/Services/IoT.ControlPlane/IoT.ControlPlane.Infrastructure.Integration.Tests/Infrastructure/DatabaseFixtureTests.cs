using IoT.ControlPlane.Infrastructure.Integration.Tests.Fixture;
using IoT.ControlPlane.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.Infrastructure;

public sealed class DatabaseFixtureTests
    : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _postgres;

    public DatabaseFixtureTests(PostgreSqlFixture postgres)
    {
        _postgres = postgres;
    }

    [Fact]
    public async Task Database_ShouldApplyMigrations()
    {
        var options = new DbContextOptionsBuilder<ControlPlaneDbContext>()
            .UseNpgsql(_postgres.ConnectionString)
            .Options;

        await using var dbContext = new ControlPlaneDbContext(options);

        await dbContext.Database.MigrateAsync();

        var appliedMigrations = await dbContext.Database
            .GetAppliedMigrationsAsync();

        Assert.NotEmpty(appliedMigrations);
    }
}
