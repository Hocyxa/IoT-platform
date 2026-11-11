using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;

public sealed record MonitoringRuleDto(
    Guid Id,
    Guid DeviceId,
    string Name,
    string? Description,
    RuleSeverity Severity,
    bool IsEnabled,
    MeasurementType? MeasurementType,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<MonitoringRuleSensorDto> Sensors,
    ConditionGroupDto RootConditionGroup
);

public sealed record MonitoringRuleSensorDto(
    Guid SensorId
);

public sealed record ConditionGroupDto(
    Guid Id,
    LogicalOperator LogicalOperator,
    IReadOnlyList<ConditionNodeDto> Nodes
);

public sealed record ConditionNodeDto(
    Guid Id,
    ConditionNodeType Type,
    LogicalOperator? LogicalOperator,
    ComparisonOperator? ComparisonOperator,
    decimal? Threshold,
    IReadOnlyList<ConditionNodeDto> Nodes
);

public enum ConditionNodeType
{
    Group = 0,
    Condition = 1
}
