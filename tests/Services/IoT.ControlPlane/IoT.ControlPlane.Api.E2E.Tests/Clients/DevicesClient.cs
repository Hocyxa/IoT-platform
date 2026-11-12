
using IoT.ControlPlane.Api.E2E.Tests.Contracts;

namespace IoT.ControlPlane.Api.E2E.Tests.Clients;

public sealed class DevicesClient(HttpClient httpClient)
    : ApiClientBase(httpClient)
{
    private const string BaseUrl = "/api/v1/devices";

    public Task<DeviceResponse> CreateAsync(
        CreateDeviceRequest request,
        CancellationToken ct = default) =>
        SendAsync<DeviceResponse>(
            HttpMethod.Post, BaseUrl, request, ct);

    public Task<DeviceResponse> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendAsync<DeviceResponse>(
            HttpMethod.Get, $"{BaseUrl}/{id}", cancellationToken: ct);

    public Task<IReadOnlyList<DeviceResponse>> GetBySiteAsync(
        Guid siteId,
        CancellationToken ct = default) =>
        SendAsync<IReadOnlyList<DeviceResponse>>(
            HttpMethod.Get,
            $"/api/v1/sites/{siteId}/devices",
            cancellationToken: ct);

    public Task<DeviceResponse> UpdateAsync(
        Guid id,
        UpdateDeviceRequest request,
        CancellationToken ct = default) =>
        SendAsync<DeviceResponse>(
            HttpMethod.Patch, $"{BaseUrl}/{id}", request, ct);

    public Task<DeviceResponse> ChangeStatusAsync(
        Guid id,
        ChangeDeviceStatusRequest request,
        CancellationToken ct = default) =>
        SendAsync<DeviceResponse>(
            HttpMethod.Patch, $"{BaseUrl}/{id}/status", request, ct);

    public Task DeleteAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Delete, $"{BaseUrl}/{id}", cancellationToken: ct);
}
