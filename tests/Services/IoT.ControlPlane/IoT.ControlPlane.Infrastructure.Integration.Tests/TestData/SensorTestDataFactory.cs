using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.TestData;

public static class SensorTestDataFactory
{
    public static Sensor Create(
        TimeProvider timeProvider,
        Guid deviceId,
        MeasurementType measurementType,
        string? name = null,
        string? code = null,
        string? unit = null)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);

        var sensorName = name ?? $"{measurementType} sensor";
        var sensorCode = code ?? $"{measurementType.ToString().ToUpperInvariant()}-001";
        var sensorUnit = unit ?? GetDefaultUnit(measurementType);

        return Sensor.Create(
            Guid.NewGuid(),
            deviceId,
            sensorName,
            sensorCode,
            measurementType,
            sensorUnit,
            timeProvider.GetUtcNow());
    }

    private static string GetDefaultUnit(MeasurementType measurementType)
    {
        return measurementType switch
        {
            MeasurementType.Temperature => "°C",
            MeasurementType.Humidity => "%",
            MeasurementType.Pressure => "Pa",
            MeasurementType.Voltage => "V",
            MeasurementType.Current => "A",
            MeasurementType.Power => "W",
            MeasurementType.Frequency => "Hz",
            MeasurementType.Speed => "m/s",
            MeasurementType.Vibration => "mm/s",
            MeasurementType.Level => "m",
            _ => throw new ArgumentOutOfRangeException(
                nameof(measurementType),
                measurementType,
                "Unsupported measurement type.")
        };
    }
}
