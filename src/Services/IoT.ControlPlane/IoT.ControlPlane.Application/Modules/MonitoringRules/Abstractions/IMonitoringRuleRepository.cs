
using IoT.ControlPlane.Domain.Modules.MonitoringRules;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;

public interface IMonitoringRuleRepository
{
    Task AddAsync(
        MonitoringRule rule,
        CancellationToken cancellationToken);

    Task<MonitoringRule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MonitoringRule>> GetByDeviceIdAsync(
        Guid deviceId,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        MonitoringRule rule,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        MonitoringRule rule,
        CancellationToken cancellationToken);
}
