namespace IoT.ControlPlane.Application.Modules.Sensors.Contracts;

public interface ISensorDeviceExistenceChecker
{
    Task<bool> ExistsAsync(
        Guid deviceId,
        CancellationToken cancellationToken);
}
