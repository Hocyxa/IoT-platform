using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Application.Modules.Sensors.DTOs;

public sealed record SensorDto(
    Guid Id,
    Guid DeviceId,
    string Name,
    string Code,
    MeasurementType Type,
    string Unit,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc
);
