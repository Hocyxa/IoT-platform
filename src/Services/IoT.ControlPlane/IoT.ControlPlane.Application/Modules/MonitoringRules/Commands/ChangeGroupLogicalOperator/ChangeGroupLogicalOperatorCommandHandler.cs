using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.ChangeGroupLogicalOperator;

public sealed class ChangeGroupLogicalOperatorCommandHandler(
    IMonitoringRuleRepository monitoringRuleRepository,
    TimeProvider timeProvider
) : IRequestHandler<ChangeGroupLogicalOperatorCommand>
{
    public async Task Handle(
        ChangeGroupLogicalOperatorCommand request,
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

        rule.ChangeGroupLogicalOperator(
            request.ConditionGroupId,
            request.LogicalOperator,
            timeProvider.GetUtcNow());

        await monitoringRuleRepository.UpdateAsync(
            rule,
            cancellationToken);
    }
}
