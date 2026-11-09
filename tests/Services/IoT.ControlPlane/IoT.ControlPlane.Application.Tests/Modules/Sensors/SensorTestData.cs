using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Application.Tests.Modules.Sensors;

internal static class SensorTestData
{
    public static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 11, 9, 10, 0, 0, TimeSpan.Zero);

    public static Sensor CreateSensor(
        Guid? id = null,
        Guid? deviceId = null,
        string name = "Temperature sensor",
        string code = "TEMP-01",
        MeasurementType measurementType = MeasurementType.Temperature,
        string unit = "°C")
    {
        return Sensor.Create(
            id ?? Guid.NewGuid(),
            deviceId ?? Guid.NewGuid(),
            name,
            code,
            measurementType,
            unit,
            CreatedAtUtc);
    }
}
