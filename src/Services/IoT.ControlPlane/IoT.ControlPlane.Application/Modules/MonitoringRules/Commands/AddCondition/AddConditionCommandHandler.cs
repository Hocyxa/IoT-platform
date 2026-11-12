
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.AddCondition;

public sealed class AddConditionCommandHandler(
    IMonitoringRuleRepository monitoringRuleRepository,
    TimeProvider timeProvider)
    : IRequestHandler<AddConditionCommand>
{
    public async Task Handle(
        AddConditionCommand request,
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

        rule.AddCondition(
            request.ConditionGroupId,
            request.ComparisonOperator,
            request.Threshold,
            timeProvider.GetUtcNow());

        await monitoringRuleRepository.UpdateAsync(
            rule,
            cancellationToken);
    }
}
