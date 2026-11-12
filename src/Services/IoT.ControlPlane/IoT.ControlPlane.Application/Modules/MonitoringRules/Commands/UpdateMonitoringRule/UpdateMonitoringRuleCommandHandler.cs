
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.UpdateMonitoringRule;

public sealed class UpdateMonitoringRuleCommandHandler(
    IMonitoringRuleRepository monitoringRuleRepository,
    TimeProvider timeProvider
    ) : IRequestHandler<UpdateMonitoringRuleCommand>
{
    public async Task Handle(UpdateMonitoringRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await monitoringRuleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (rule is null)
        {
            throw new InvalidOperationException(
                $"Monitoring rule with ID '{request.Id}' does not exist.");

        }
        rule.UpdateDetails(request.Name, request.Description, request.Severity, timeProvider.GetUtcNow());

        await monitoringRuleRepository.UpdateAsync(rule, cancellationToken);
    }
}
