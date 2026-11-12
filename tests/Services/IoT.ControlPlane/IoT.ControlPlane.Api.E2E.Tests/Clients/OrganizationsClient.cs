
using IoT.ControlPlane.Api.E2E.Tests.Contracts;

namespace IoT.ControlPlane.Api.E2E.Tests.Clients;

public sealed class OrganizationsClient(HttpClient httpClient)
    : ApiClientBase(httpClient)
{
    private const string BaseUrl = "/api/v1/organizations";

    public Task<OrganizationResponse> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken ct = default) =>
        SendAsync<OrganizationResponse>(
            HttpMethod.Post, BaseUrl, request, ct);

    public Task<IReadOnlyList<OrganizationResponse>> GetAllAsync(
        CancellationToken ct = default) =>
        SendAsync<IReadOnlyList<OrganizationResponse>>(
            HttpMethod.Get, BaseUrl, cancellationToken: ct);

    public Task<OrganizationResponse> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendAsync<OrganizationResponse>(
            HttpMethod.Get, $"{BaseUrl}/{id}", cancellationToken: ct);

    public Task RenameAsync(
        Guid id,
        RenameOrganizationRequest request,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Patch, $"{BaseUrl}/{id}", request, ct);

    public Task DeleteAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Delete, $"{BaseUrl}/{id}", cancellationToken: ct);
}
