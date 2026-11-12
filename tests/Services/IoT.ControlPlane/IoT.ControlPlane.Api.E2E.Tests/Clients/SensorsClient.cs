
using IoT.ControlPlane.Api.E2E.Tests.Contracts;

namespace IoT.ControlPlane.Api.E2E.Tests.Clients;

public sealed class SensorsClient(HttpClient httpClient)
    : ApiClientBase(httpClient)
{
    private const string BaseUrl = "/api/v1/sensors";

    public Task<SensorResponse> CreateAsync(
        CreateSensorRequest request,
        CancellationToken ct = default) =>
        SendAsync<SensorResponse>(
            HttpMethod.Post, BaseUrl, request, ct);

    public Task<SensorResponse> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendAsync<SensorResponse>(
            HttpMethod.Get, $"{BaseUrl}/{id}", cancellationToken: ct);

    public Task<IReadOnlyList<SensorResponse>> GetAllAsync(
        CancellationToken ct = default) =>
        SendAsync<IReadOnlyList<SensorResponse>>(
            HttpMethod.Get, BaseUrl, cancellationToken: ct);

    public Task<IReadOnlyList<SensorResponse>> GetByDeviceAsync(
        Guid deviceId,
        CancellationToken ct = default) =>
        SendAsync<IReadOnlyList<SensorResponse>>(
            HttpMethod.Get,
            $"/api/v1/devices/{deviceId}/sensors",
            cancellationToken: ct);

    public Task UpdateAsync(
        Guid id,
        UpdateSensorRequest request,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Put, $"{BaseUrl}/{id}", request, ct);

    public Task DeleteAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Delete, $"{BaseUrl}/{id}", cancellationToken: ct);
}
