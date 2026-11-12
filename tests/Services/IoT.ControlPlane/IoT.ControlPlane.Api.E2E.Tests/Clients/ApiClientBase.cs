
using System.Net.Http.Json;

using IoT.ControlPlane.Api.E2E.Tests.Contracts;

namespace IoT.ControlPlane.Api.E2E.Tests.Clients;

public abstract class ApiClientBase(HttpClient httpClient)
{
    protected async Task<TResponse> SendAsync<TResponse>(
        HttpMethod method,
        string uri,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, uri);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<TResponse>(
            cancellationToken: cancellationToken);

        return result ?? throw new InvalidOperationException(
            $"API returned an empty response for {method} {uri}.");
    }

    protected async Task SendNoContentAsync(
        HttpMethod method,
        string uri,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, uri);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);
    }

    protected async Task<Guid> SendCreateAsync(
        string uri,
        object body,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = JsonContent.Create(body)
        };

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        // В OpenAPI тело ответа не описано.
        // Поддерживаем { id } и Location header.
        var content = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(content))
        {
            var result = System.Text.Json.JsonSerializer.Deserialize<
                CreatedIdResponse>(
                content,
                new System.Text.Json.JsonSerializerOptions(
                    System.Text.Json.JsonSerializerDefaults.Web));

            if (result is not null && result.Id != Guid.Empty)
            {
                return result.Id;
            }
        }

        var location = response.Headers.Location;

        if (location is not null)
        {
            var segment = location.Segments.LastOrDefault()?.Trim('/');

            if (Guid.TryParse(segment, out var id))
            {
                return id;
            }
        }

        throw new InvalidOperationException(
            $"API created resource at {uri}, but returned no resource ID.");
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync(
            cancellationToken);

        throw new HttpRequestException(
            $"API request failed. " +
            $"Status: {(int)response.StatusCode} " +
            $"{response.StatusCode}. Body: {content}",
            null,
            response.StatusCode);
    }
}
