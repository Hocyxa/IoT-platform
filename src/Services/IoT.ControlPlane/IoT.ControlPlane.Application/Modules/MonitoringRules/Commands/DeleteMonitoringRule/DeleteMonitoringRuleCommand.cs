
using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.DeleteMonitoringRule;

public sealed record DeleteMonitoringRuleCommand(
    Guid Id
) : IRequest;
