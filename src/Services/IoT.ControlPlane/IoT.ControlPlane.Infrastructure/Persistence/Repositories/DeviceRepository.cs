
using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Contracts;
using IoT.ControlPlane.Application.Modules.Sensors.Contracts;
using IoT.ControlPlane.Domain.Modules.Devices;

using Microsoft.EntityFrameworkCore;

namespace IoT.ControlPlane.Infrastructure.Persistence.Repositories;

public sealed class DeviceRepository(ControlPlaneDbContext dbContext) : IDeviceRepository, ISensorDeviceExistenceChecker, IDeviceExistenceChecker
{
    public async Task AddAsync(Device device, CancellationToken cancellationToken)
    {
        await dbContext.Devices.AddAsync(device, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Device device, CancellationToken cancellationToken)
    {
        dbContext.Devices.Remove(device);
        await dbContext.SaveChangesAsync(cancellationToken);

    }

    public async Task<IReadOnlyList<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Devices.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Devices.FirstOrDefaultAsync(device => device.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Device>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken)
    {
        return await dbContext.Devices.AsNoTracking().Where(device => device.SiteId == siteId).ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Device device, CancellationToken cancellationToken)
    {
        dbContext.Devices.Update(device);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<bool> ExistsAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        return await dbContext.Devices.AnyAsync(device => device.Id == deviceId, cancellationToken);
    }
}
