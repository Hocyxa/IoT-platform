namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Contracts;

public interface IDeviceExistenceChecker
{
    Task<bool> ExistsAsync(
        Guid deviceId,
        CancellationToken cancellationToken);
}
