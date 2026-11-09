using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.UpdateSensor;

public sealed class UpdateSensorHandler(
    ISensorRepository sensorRepository,
    TimeProvider timeProvider
) : IRequestHandler<UpdateSensorCommand, bool>
{
    public async Task<bool> Handle(
        UpdateSensorCommand request,
        CancellationToken cancellationToken)
    {
        var sensor = await sensorRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (sensor is null)
        {
            return false;
        }

        sensor.UpdateDetails(
            request.Name,
            request.MeasurementType,
            request.Unit,
            timeProvider.GetUtcNow());

        await sensorRepository.UpdateAsync(sensor, cancellationToken);

        return true;
    }
}
