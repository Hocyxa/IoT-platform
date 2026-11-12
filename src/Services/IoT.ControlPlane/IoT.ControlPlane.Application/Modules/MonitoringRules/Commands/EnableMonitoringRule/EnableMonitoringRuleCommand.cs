using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.EnableMonitoringRule;

public sealed record EnableMonitoringRuleCommand(
    Guid MonitoringRuleId
) : IRequest;
