namespace IoT.ControlPlane.Application.Modules.Sensors.Contracts;

public interface IDevicexistenceChecker
{
    Task<bool> ExistsAsync(
        Guid deviceId,
        CancellationToken cancellationToken);
}
