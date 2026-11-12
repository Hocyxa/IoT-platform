using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.DisableMonitoringRule;

public sealed record DisableMonitoringRuleCommand(
    Guid MonitoringRuleId
) : IRequest;
