
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.UpdateMonitoringRule;

public sealed record UpdateMonitoringRuleCommand(
    Guid Id,
    string Name,
    string? Description,
    RuleSeverity Severity
) : IRequest;
