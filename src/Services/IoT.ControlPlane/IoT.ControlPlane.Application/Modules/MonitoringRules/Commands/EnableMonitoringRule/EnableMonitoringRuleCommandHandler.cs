using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.EnableMonitoringRule;

public sealed class EnableMonitoringRuleCommandHandler(
    IMonitoringRuleRepository monitoringRuleRepository,
    TimeProvider timeProvider
) : IRequestHandler<EnableMonitoringRuleCommand>
{
    public async Task Handle(
        EnableMonitoringRuleCommand request,
        CancellationToken cancellationToken)
    {
        var rule = await monitoringRuleRepository.GetByIdAsync(
            request.MonitoringRuleId,
            cancellationToken);

        if (rule is null)
        {
            throw new InvalidOperationException(
                $"Monitoring rule with ID '{request.MonitoringRuleId}' was not found.");
        }

        rule.Enable(timeProvider.GetUtcNow());

        await monitoringRuleRepository.UpdateAsync(
            rule,
            cancellationToken);
    }
}
