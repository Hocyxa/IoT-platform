using ApiMeasurementType =
    IoT.ControlPlane.Api.Contracts.Sensors.MeasurementTypeRequest;

using DomainMeasurementType =
    IoT.ControlPlane.Domain.Modules.Sensors.MeasurementType;

namespace IoT.ControlPlane.Api.Contracts.Sensors;

public static class MeasurementTypeMapper
{
    public static DomainMeasurementType ToDomain(ApiMeasurementType type)
    {
        return type switch
        {
            ApiMeasurementType.Temperature => DomainMeasurementType.Temperature,
            ApiMeasurementType.Humidity => DomainMeasurementType.Humidity,
            ApiMeasurementType.Pressure => DomainMeasurementType.Pressure,
            ApiMeasurementType.Voltage => DomainMeasurementType.Voltage,
            ApiMeasurementType.Current => DomainMeasurementType.Current,
            ApiMeasurementType.Power => DomainMeasurementType.Power,
            ApiMeasurementType.Frequency => DomainMeasurementType.Frequency,
            ApiMeasurementType.Speed => DomainMeasurementType.Speed,
            ApiMeasurementType.Vibration => DomainMeasurementType.Vibration,
            ApiMeasurementType.Level => DomainMeasurementType.Level,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    public static ApiMeasurementType ToApi(DomainMeasurementType type)
    {
        return type switch
        {
            DomainMeasurementType.Temperature => ApiMeasurementType.Temperature,
            DomainMeasurementType.Humidity => ApiMeasurementType.Humidity,
            DomainMeasurementType.Pressure => ApiMeasurementType.Pressure,
            DomainMeasurementType.Voltage => ApiMeasurementType.Voltage,
            DomainMeasurementType.Current => ApiMeasurementType.Current,
            DomainMeasurementType.Power => ApiMeasurementType.Power,
            DomainMeasurementType.Frequency => ApiMeasurementType.Frequency,
            DomainMeasurementType.Speed => ApiMeasurementType.Speed,
            DomainMeasurementType.Vibration => ApiMeasurementType.Vibration,
            DomainMeasurementType.Level => ApiMeasurementType.Level,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
