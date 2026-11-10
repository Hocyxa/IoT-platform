
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.UpdateCondition;

public sealed record UpdateConditionCommand(
    Guid MonitoringRuleId,
    Guid ConditionId,
    ComparisonOperator ComparisonOperator,
    decimal Threshold
) : IRequest;
