using IoT.ControlPlane.Application.Modules.Sensors.DTOs;

namespace IoT.ControlPlane.Api.Contracts.Sensors;

public static class SensorResponseMapper
{
    public static SensorResponse ToResponse(SensorDto sensor)
    {
        return new SensorResponse(
            sensor.Id,
            sensor.DeviceId,
            sensor.Name,
            sensor.Code,
            MeasurementTypeMapper.ToApi(sensor.Type).ToString(),
            sensor.Unit,
            sensor.CreatedAtUtc,
            sensor.UpdatedAtUtc);
    }
}
