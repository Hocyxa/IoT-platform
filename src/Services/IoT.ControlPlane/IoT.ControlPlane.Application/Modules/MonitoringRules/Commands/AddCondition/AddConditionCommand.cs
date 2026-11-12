
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.AddCondition;

public sealed record AddConditionCommand(
    Guid MonitoringRuleId,
    Guid ConditionGroupId,
    ComparisonOperator ComparisonOperator,
    decimal Threshold
) : IRequest;
