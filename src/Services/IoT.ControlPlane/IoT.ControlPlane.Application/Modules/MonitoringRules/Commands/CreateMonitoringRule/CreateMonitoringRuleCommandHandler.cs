
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Contracts;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.CreateMonitoringRule;

public sealed class CreateMonitoringRuleCommandHandler(
    IMonitoringRuleRepository monitoringRuleRepository,
    IDeviceExistenceChecker deviceExistenceChecker,
    TimeProvider timeProvider)
    : IRequestHandler<CreateMonitoringRuleCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateMonitoringRuleCommand request,
        CancellationToken cancellationToken)
    {
        var deviceExists = await deviceExistenceChecker.ExistsAsync(
            request.DeviceId,
            cancellationToken);

        if (!deviceExists)
        {
            throw new InvalidOperationException(
                $"Device with ID '{request.DeviceId}' does not exist.");
        }

        var rule = MonitoringRule.Create(
            request.DeviceId,
            request.Name,
            request.Description,
            request.Severity,
            timeProvider.GetUtcNow());

        await monitoringRuleRepository.AddAsync(
            rule,
            cancellationToken);

        return rule.Id;
    }
}
