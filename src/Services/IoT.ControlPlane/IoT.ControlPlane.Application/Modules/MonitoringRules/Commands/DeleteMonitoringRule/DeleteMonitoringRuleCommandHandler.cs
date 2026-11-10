
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.DeleteMonitoringRule;

public sealed class DeleteMonitoringRuleCommandHandler(
    IMonitoringRuleRepository monitoringRuleRepository)
    : IRequestHandler<DeleteMonitoringRuleCommand>
{
    public async Task Handle(
        DeleteMonitoringRuleCommand request,
        CancellationToken cancellationToken)
    {
        var rule = await monitoringRuleRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (rule is null)
        {
            throw new InvalidOperationException(
                $"Monitoring rule with ID '{request.Id}' does not exist.");
        }

        await monitoringRuleRepository.DeleteAsync(
            rule,
            cancellationToken);
    }
}
