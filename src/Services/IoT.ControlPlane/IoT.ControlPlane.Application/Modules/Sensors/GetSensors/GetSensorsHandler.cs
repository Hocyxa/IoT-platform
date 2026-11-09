using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.GetSensors;

public sealed class GetSensorsHandler(ISensorRepository repository) : IRequestHandler<GetSensorsQuery, IReadOnlyList<SensorDto>>
{
    public async Task<IReadOnlyList<SensorDto>> Handle(GetSensorsQuery request, CancellationToken cancellationToken)
    {
        var sensors = await repository.GetAllAsync(cancellationToken);

        return sensors
        .Select(sensor => new SensorDto(
                sensor.Id,
                sensor.DeviceId,
                sensor.Name,
                sensor.Code,
                sensor.MeasurementType,
                sensor.Unit,
                sensor.CreatedAtUtc,
                sensor.UpdatedAtUtc))
        .ToList();
    }
}
