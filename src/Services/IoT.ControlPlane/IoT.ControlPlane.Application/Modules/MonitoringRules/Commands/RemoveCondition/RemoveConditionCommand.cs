using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.RemoveCondition;

public sealed record RemoveConditionCommand(
    Guid MonitoringRuleId,
    Guid ConditionId
) : IRequest;
