using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.GetSensorById;

public sealed class GetSensorByIdHandler(ISensorRepository repository) : IRequestHandler<GetSensorByIdQuery, SensorDto?>
{
    public async Task<SensorDto?> Handle(GetSensorByIdQuery request, CancellationToken cancellationToken)
    {
        var sensor = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (sensor is null)
        {
            return null;
        }

        return new SensorDto(
            sensor.Id,
            sensor.DeviceId,
            sensor.Name,
            sensor.Code,
            sensor.Type,
            sensor.Unit,
            sensor.CreatedAtUtc,
            sensor.UpdatedAtUtc
        );
    }
}
