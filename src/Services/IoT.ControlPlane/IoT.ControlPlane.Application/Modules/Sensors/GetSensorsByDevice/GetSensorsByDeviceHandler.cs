
using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.GetSensorsByDevice;


public sealed class GetSensorsByDeviceIdHandler(ISensorRepository repository) : IRequestHandler<GetSensorsByDeviceQuery, IReadOnlyCollection<SensorDto>>
{
    public async Task<IReadOnlyCollection<SensorDto>> Handle(GetSensorsByDeviceQuery request, CancellationToken cancellationToken)
    {
        var sensors = await repository.GetByDeviceIdAsync(request.DeviceId, cancellationToken);

        return sensors
            .Select(sensor => new SensorDto(
                sensor.Id,
                sensor.DeviceId,
                sensor.Name,
                sensor.Code,
                sensor.MeasurementType,
                sensor.Unit,
                sensor.CreatedAtUtc,
                sensor.UpdatedAtUtc
            ))
            .ToList();
    }
}
