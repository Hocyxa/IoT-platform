using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Application.Modules.Sensors.Abstractions;

public interface ISensorRepository
{
    Task AddAsync(
        Sensor sensor,
        CancellationToken cancellationToken);
    Task<Sensor?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Sensor>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Sensor>> GetByDeviceIdAsync(
        Guid deviceId,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Sensor sensor,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Sensor sensor,
        CancellationToken cancellationToken);
}
