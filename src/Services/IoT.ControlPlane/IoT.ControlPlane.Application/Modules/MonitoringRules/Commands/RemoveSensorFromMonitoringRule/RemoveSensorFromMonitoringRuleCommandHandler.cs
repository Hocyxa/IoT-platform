
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.RemoveSensorFromMonitoringRule;

public sealed class RemoveSensorFromMonitoringRuleCommandHandler(
    IMonitoringRuleRepository monitoringRuleRepository,
    TimeProvider timeProvider)
    : IRequestHandler<RemoveSensorFromMonitoringRuleCommand>
{
    public async Task Handle(
        RemoveSensorFromMonitoringRuleCommand request,
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

        rule.RemoveSensor(
            request.SensorId,
            timeProvider.GetUtcNow());

        await monitoringRuleRepository.UpdateAsync(
            rule,
            cancellationToken);
    }
}
