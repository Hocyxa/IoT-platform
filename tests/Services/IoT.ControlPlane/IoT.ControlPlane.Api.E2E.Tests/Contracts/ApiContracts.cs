
namespace IoT.ControlPlane.Api.E2E.Tests.Contracts;

// Organizations

public sealed record CreateOrganizationRequest(string Name);

public sealed record RenameOrganizationRequest(string Name);

public sealed record OrganizationResponse(
    Guid Id,
    string? Name,
    DateTimeOffset CreatedAtUtc);

// Sites

public sealed record CreateSiteRequest(
    Guid OrganizationId,
    string Name,
    string? Address);

public sealed record UpdateSiteRequest(
    string Name,
    string? Address);

public sealed record SiteResponse(
    Guid Id,
    Guid OrganizationId,
    string? Name,
    string? Address,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

// Devices

public sealed record CreateDeviceRequest(
    Guid SiteId,
    string Name,
    string SerialNumber);

public sealed record UpdateDeviceRequest(string Name);

public sealed record DeviceResponse(
    Guid Id,
    Guid SiteId,
    string? Name,
    string? SerialNumber,
    string? Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record ChangeDeviceStatusRequest(int Status);

// Sensors

public sealed record CreateSensorRequest(
    Guid DeviceId,
    string Name,
    string Code,
    MeasurementTypeRequest MeasurementType,
    string Unit);

public sealed record UpdateSensorRequest(
    string Name,
    MeasurementTypeRequest MeasurementType,
    string Unit);

public sealed record SensorResponse(
    Guid Id,
    Guid DeviceId,
    string? Name,
    string? Code,
    string? MeasurementType,
    string? Unit,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

// Monitoring rules

public sealed record CreateMonitoringRuleRequest(
    Guid DeviceId,
    string Name,
    string? Description,
    RuleSeverityRequest Severity);

public sealed record UpdateMonitoringRuleRequest(
    string Name,
    string? Description,
    RuleSeverityRequest Severity);

public sealed record CreatedIdResponse(Guid Id);

public sealed record MonitoringRuleDto(
    Guid Id,
    Guid DeviceId,
    string Name,
    string? Description,
    RuleSeverityRequest Severity,
    bool IsEnabled,
    MeasurementTypeRequest? MeasurementType,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<MonitoringRuleSensorDto> Sensors,
    ConditionGroupDto RootConditionGroup);

public sealed record MonitoringRuleSensorDto(Guid SensorId);

public sealed record ConditionGroupDto(
    Guid Id,
    LogicalOperatorRequest LogicalOperator,
    IReadOnlyList<ConditionNodeDto> Nodes);

public sealed record ConditionNodeDto(
    Guid Id,
    ConditionNodeType Type,
    LogicalOperatorRequest? LogicalOperator,
    ComparisonOperatorRequest? ComparisonOperator,
    decimal? Threshold,
    IReadOnlyList<ConditionNodeDto> Nodes);

public enum ConditionNodeType
{
    Group = 0,
    Condition = 1
}

public sealed record AddConditionRequest(
    ComparisonOperatorRequest ComparisonOperator,
    decimal Threshold);

public sealed record UpdateConditionRequest(
    ComparisonOperatorRequest ComparisonOperator,
    decimal Threshold);

public sealed record ChangeGroupLogicalOperatorRequest(
    LogicalOperatorRequest LogicalOperator);

// Enums

public enum MeasurementTypeRequest
{
    Temperature = 0,
    Humidity = 1,
    Pressure = 2,
    Voltage = 3,
    Current = 4,
    Power = 5,
    Frequency = 6,
    Speed = 7,
    Vibration = 8,
    Level = 9
}

public enum RuleSeverityRequest
{
    Information = 0,
    Warning = 1,
    Critical = 2
}

public enum ComparisonOperatorRequest
{
    GreaterThan = 0,
    GreaterThanOrEqual = 1,
    LessThan = 2,
    LessThanOrEqual = 3,
    Equal = 4,
    NotEqual = 5
}

public enum LogicalOperatorRequest
{
    And = 0,
    Or = 1
}

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
