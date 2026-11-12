
using IoT.ControlPlane.Api.E2E.Tests.Contracts;

namespace IoT.ControlPlane.Api.E2E.Tests.Clients;

public sealed class SitesClient(HttpClient httpClient)
    : ApiClientBase(httpClient)
{
    private const string BaseUrl = "/api/v1/sites";

    public Task<SiteResponse> CreateAsync(
        CreateSiteRequest request,
        CancellationToken ct = default) =>
        SendAsync<SiteResponse>(
            HttpMethod.Post, BaseUrl, request, ct);

    public Task<IReadOnlyList<SiteResponse>> GetAllAsync(
        CancellationToken ct = default) =>
        SendAsync<IReadOnlyList<SiteResponse>>(
            HttpMethod.Get, BaseUrl, cancellationToken: ct);

    public Task<SiteResponse> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendAsync<SiteResponse>(
            HttpMethod.Get, $"{BaseUrl}/{id}", cancellationToken: ct);

    public Task<IReadOnlyList<SiteResponse>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken ct = default) =>
        SendAsync<IReadOnlyList<SiteResponse>>(
            HttpMethod.Get,
            $"/api/v1/organizations/{organizationId}/sites",
            cancellationToken: ct);

    public Task UpdateAsync(
        Guid id,
        UpdateSiteRequest request,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Put, $"{BaseUrl}/{id}", request, ct);

    public Task DeleteAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Delete, $"{BaseUrl}/{id}", cancellationToken: ct);
}
