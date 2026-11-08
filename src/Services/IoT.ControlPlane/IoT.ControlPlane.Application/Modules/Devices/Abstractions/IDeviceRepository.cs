using IoT.ControlPlane.Domain.Modules.Devices;

namespace IoT.ControlPlane.Application.Modules.Devices.Abstractions;

public interface IDeviceRepository
{
    Task AddAsync(
        Device device,
        CancellationToken cancellationToken);
    Task<Device?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Device>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Device>> GetBySiteIdAsync(
        Guid siteId,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Device device,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Device device,
        CancellationToken cancellationToken);
}
