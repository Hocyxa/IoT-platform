
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.AssignSensorToMonitoringRule;

public sealed class AssignSensorToMonitoringRuleCommandHandler(
    IMonitoringRuleRepository monitoringRuleRepository,
    ISensorRepository sensorRepository,
    TimeProvider timeProvider)
    : IRequestHandler<AssignSensorToMonitoringRuleCommand>
{
    public async Task Handle(
        AssignSensorToMonitoringRuleCommand request,
        CancellationToken cancellationToken)
    {
        var rule = await monitoringRuleRepository.GetByIdAsync(
            request.MonitoringRuleId,
            cancellationToken);

        if (rule is null)
        {
            throw new InvalidOperationException(
                $"Monitoring rule with ID '{request.MonitoringRuleId}' does not exist.");
        }

        var sensor = await sensorRepository.GetByIdAsync(
            request.SensorId,
            cancellationToken);

        if (sensor is null)
        {
            throw new InvalidOperationException(
                $"Sensor with ID '{request.SensorId}' does not exist.");
        }

        rule.AssignSensor(
            sensor.Id,
            sensor.DeviceId,
            sensor.Type,
            timeProvider.GetUtcNow());

        await monitoringRuleRepository.UpdateAsync(
            rule,
            cancellationToken);
    }
}
