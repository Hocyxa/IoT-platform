
using IoT.ControlPlane.Api.E2E.Tests.Contracts;

namespace IoT.ControlPlane.Api.E2E.Tests.Clients;

public sealed class MonitoringRulesClient(HttpClient httpClient)
    : ApiClientBase(httpClient)
{
    private const string BaseUrl = "/api/v1/monitoringrules";

    public Task<Guid> CreateAsync(
        CreateMonitoringRuleRequest request,
        CancellationToken ct = default) =>
        SendCreateAsync(BaseUrl, request, ct);

    public Task<MonitoringRuleDto> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendAsync<MonitoringRuleDto>(
            HttpMethod.Get, $"{BaseUrl}/{id}", cancellationToken: ct);

    public Task<IReadOnlyList<MonitoringRuleDto>> GetByDeviceAsync(
        Guid deviceId,
        CancellationToken ct = default) =>
        SendAsync<IReadOnlyList<MonitoringRuleDto>>(
            HttpMethod.Get,
            $"/api/v1/devices/{deviceId}/monitoringrules",
            cancellationToken: ct);

    public Task<PagedResult<MonitoringRuleDto>> GetAllAsync(
        int page = 1,
        int pageSize = 20,
        Guid? deviceId = null,
        RuleSeverityRequest? severity = null,
        bool? isEnabled = null,
        MeasurementTypeRequest? measurementType = null,
        CancellationToken ct = default)
    {
        var query = new List<string>
        {
            $"Page={page}",
            $"PageSize={pageSize}"
        };

        if (deviceId.HasValue)
            query.Add($"DeviceId={deviceId}");

        if (severity.HasValue)
            query.Add($"Severity={(int)severity.Value}");

        if (isEnabled.HasValue)
            query.Add($"IsEnabled={isEnabled.Value.ToString().ToLowerInvariant()}");

        if (measurementType.HasValue)
            query.Add($"MeasurementType={(int)measurementType.Value}");

        var uri = $"{BaseUrl}?{string.Join("&", query)}";

        return SendAsync<PagedResult<MonitoringRuleDto>>(
            HttpMethod.Get, uri, cancellationToken: ct);
    }

    public Task UpdateAsync(
        Guid id,
        UpdateMonitoringRuleRequest request,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Put, $"{BaseUrl}/{id}", request, ct);

    public Task DeleteAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Delete, $"{BaseUrl}/{id}", cancellationToken: ct);

    public Task EnableAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Patch, $"{BaseUrl}/{id}/enable",
            cancellationToken: ct);

    public Task DisableAsync(
        Guid id,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Patch, $"{BaseUrl}/{id}/disable",
            cancellationToken: ct);

    public Task AssignSensorAsync(
        Guid ruleId,
        Guid sensorId,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Post,
            $"{BaseUrl}/{ruleId}/sensors/{sensorId}",
            cancellationToken: ct);

    public Task RemoveSensorAsync(
        Guid ruleId,
        Guid sensorId,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Delete,
            $"{BaseUrl}/{ruleId}/sensors/{sensorId}",
            cancellationToken: ct);

    public Task AddConditionAsync(
        Guid ruleId,
        Guid groupId,
        AddConditionRequest request,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Post,
            $"{BaseUrl}/{ruleId}/condition-groups/{groupId}/conditions",
            request,
            ct);

    public Task UpdateConditionAsync(
        Guid ruleId,
        Guid conditionId,
        UpdateConditionRequest request,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Put,
            $"{BaseUrl}/{ruleId}/conditions/{conditionId}",
            request,
            ct);

    public Task RemoveConditionAsync(
        Guid ruleId,
        Guid conditionId,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Delete,
            $"{BaseUrl}/{ruleId}/conditions/{conditionId}",
            cancellationToken: ct);

    public Task ChangeLogicalOperatorAsync(
        Guid ruleId,
        Guid groupId,
        ChangeGroupLogicalOperatorRequest request,
        CancellationToken ct = default) =>
        SendNoContentAsync(
            HttpMethod.Patch,
            $"{BaseUrl}/{ruleId}/condition-groups/{groupId}/logical-operator",
            request,
            ct);
}
