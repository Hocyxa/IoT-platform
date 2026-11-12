
namespace IoT.ControlPlane.Api.E2E.Tests.Fixtures;

public sealed class ApiFixture : IDisposable
{
    public HttpClient Client { get; }

    public ApiFixture()
    {
        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL")
            ?? "http://localhost:18080";

        Client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}
