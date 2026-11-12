using IoT.ControlPlane.Infrastructure.Integration.Tests.Fixture;

using Npgsql;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.Infrastructure;

public sealed class PostgreSqlFixtureTests
    : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlFixtureTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task PostgreSql_ShouldBeAvailable()
    {
        await using var connection = new NpgsqlConnection(
            _fixture.ConnectionString);

        await connection.OpenAsync();

        Assert.Equal(
            System.Data.ConnectionState.Open,
            connection.State);
    }
}
