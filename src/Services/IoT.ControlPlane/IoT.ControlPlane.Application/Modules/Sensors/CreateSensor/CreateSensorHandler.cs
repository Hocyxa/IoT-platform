using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.Contracts;
using IoT.ControlPlane.Domain.Modules.Sensors;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.CreateSensor;

public sealed class CreateSensorHandler(
    ISensorRepository sensorRepository,
    ISensorDeviceExistenceChecker deviceExistenceChecker,
    TimeProvider timeProvider
) : IRequestHandler<CreateSensorCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateSensorCommand request,
        CancellationToken cancellationToken)
    {
        var deviceExists = await deviceExistenceChecker.ExistsAsync(
            request.DeviceId,
            cancellationToken);

        if (!deviceExists)
        {
            throw new KeyNotFoundException(
                $"Device with ID '{request.DeviceId}' was not found.");
        }

        var sensor = Sensor.Create(
            Guid.NewGuid(),
            request.DeviceId,
            request.Name,
            request.Code,
            request.MeasurementType,
            request.Unit,
            timeProvider.GetUtcNow());

        await sensorRepository.AddAsync(sensor, cancellationToken);

        return sensor.Id;
    }
}
