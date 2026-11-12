
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Sensors;

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
    Task<IReadOnlyList<MonitoringRule>> GetAllAsync(
        CancellationToken cancellationToken);
    Task<(IReadOnlyList<MonitoringRule> Items, int TotalCount)> GetFilteredAsync(
        Guid? deviceId,
        RuleSeverity? severity,
        bool? isEnabled,
        MeasurementType? measurementType,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
