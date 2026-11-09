namespace IoT.ControlPlane.Api.Contracts.Sensors;

public sealed record SensorResponse(
    Guid Id,
    Guid DeviceId,
    string Name,
    string Code,
    string MeasurementType,
    string Unit,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
