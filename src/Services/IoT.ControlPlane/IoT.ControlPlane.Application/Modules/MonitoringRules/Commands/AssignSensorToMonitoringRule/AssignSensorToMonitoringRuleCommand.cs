
using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.AssignSensorToMonitoringRule;

public sealed record AssignSensorToMonitoringRuleCommand(
    Guid MonitoringRuleId,
    Guid SensorId
) : IRequest;
