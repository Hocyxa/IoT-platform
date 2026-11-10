
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.CreateMonitoringRule;

public sealed record CreateMonitoringRuleCommand(
    Guid DeviceId,
    string Name,
    string? Description,
    RuleSeverity Severity
) : IRequest<Guid>;
