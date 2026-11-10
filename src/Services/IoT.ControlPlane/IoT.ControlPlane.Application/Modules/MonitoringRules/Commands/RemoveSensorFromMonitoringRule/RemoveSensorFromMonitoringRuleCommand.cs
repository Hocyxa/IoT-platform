
using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.RemoveSensorFromMonitoringRule;

public sealed record RemoveSensorFromMonitoringRuleCommand(
    Guid MonitoringRuleId,
    Guid SensorId
) : IRequest;
