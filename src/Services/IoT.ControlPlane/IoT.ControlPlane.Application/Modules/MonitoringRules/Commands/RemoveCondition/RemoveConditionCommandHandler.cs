using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.RemoveCondition;

public sealed class RemoveConditionCommandHandler(
    IMonitoringRuleRepository monitoringRuleRepository,
    TimeProvider timeProvider
) : IRequestHandler<RemoveConditionCommand>
{
    public async Task Handle(
        RemoveConditionCommand request,
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

        rule.RemoveCondition(
            request.ConditionId,
            timeProvider.GetUtcNow());

        await monitoringRuleRepository.UpdateAsync(
            rule,
            cancellationToken);
    }
}
