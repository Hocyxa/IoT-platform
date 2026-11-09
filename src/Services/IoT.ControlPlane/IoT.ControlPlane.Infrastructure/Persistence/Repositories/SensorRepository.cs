using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Domain.Modules.Sensors;

using Microsoft.EntityFrameworkCore;

namespace IoT.ControlPlane.Infrastructure.Persistence.Repositories;

public sealed class SensorRepository(ControlPlaneDbContext dbContext) : ISensorRepository
{
    public async Task AddAsync(Sensor sensor, CancellationToken cancellationToken)
    {
        await dbContext.Sensors.AddAsync(sensor, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Sensor sensor, CancellationToken cancellationToken)
    {
        dbContext.Remove(sensor);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sensor>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Sensors.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sensor>> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        return await dbContext.Sensors.Where(sensor => sensor.DeviceId == deviceId).ToListAsync(cancellationToken);
    }

    public async Task<Sensor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Sensors.FirstOrDefaultAsync(sensor => sensor.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Sensor sensor, CancellationToken cancellationToken)
    {
        dbContext.Update(sensor);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
