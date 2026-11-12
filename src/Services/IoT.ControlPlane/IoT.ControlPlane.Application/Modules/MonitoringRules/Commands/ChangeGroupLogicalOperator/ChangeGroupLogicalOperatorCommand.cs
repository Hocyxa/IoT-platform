using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.ChangeGroupLogicalOperator;

public sealed record ChangeGroupLogicalOperatorCommand(
    Guid MonitoringRuleId,
    Guid ConditionGroupId,
    LogicalOperator LogicalOperator
) : IRequest;
