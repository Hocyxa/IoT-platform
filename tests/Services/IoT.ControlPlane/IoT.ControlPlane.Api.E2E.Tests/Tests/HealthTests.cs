
using System.Net;
using System.Net.Http.Json;

using IoT.ControlPlane.Api.E2E.Tests.Infrastructure;

namespace IoT.ControlPlane.Api.E2E.Tests.Tests;

[Collection(DockerComposeCollection.Name)]
public sealed class HealthTests(
    DockerComposeFixture fixture)
{
    [Fact]
    public async Task Should_Return_Healthy_Status()
    {
        // Act
        using var response =
            await fixture.Client.GetAsync("/health");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<HealthResponse>();

        Assert.NotNull(result);
        Assert.Equal("Healthy", result.Status);
        Assert.Equal("IoT.ControlPlane", result.Service);
    }

    private sealed record HealthResponse(
        string Status,
        string Service);
}
