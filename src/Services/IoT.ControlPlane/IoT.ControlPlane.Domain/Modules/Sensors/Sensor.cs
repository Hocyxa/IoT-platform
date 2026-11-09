
using IoT.ControlPlane.Domain.Common;

namespace IoT.ControlPlane.Domain.Modules.Sensors;


public sealed class Sensor : AuditableEntity
{

    public Guid Id { get; private set; }
    public Guid DeviceId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public MeasurementType Type { get; private set; }
    public string Unit { get; private set; } = null!;
    private Sensor()
    {
    }

    private Sensor(
        Guid id,
        Guid deviceId,
        string name,
        string code,
        MeasurementType measurementType,
        string unit,
        DateTimeOffset createdAtUtc) : base(createdAtUtc)
    {
        Id = id;
        DeviceId = deviceId;
        Name = name;
        Code = code;
        Type = measurementType;
        Unit = unit;
    }

    public static Sensor Create(
        Guid id,
        Guid deviceId,
        string name,
        string code,
        MeasurementType measurementType,
        string unit,
        DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Sensor ID cannot be empty.", nameof(id));
        }
        if (deviceId == Guid.Empty)
        {
            throw new ArgumentException("Device ID cannot be empty.", nameof(deviceId));
        }

        if (!Enum.IsDefined(typeof(MeasurementType), measurementType))
        {
            throw new ArgumentOutOfRangeException(nameof(measurementType));
        }

        return new Sensor(id, deviceId, NormalizeName(name), NormalizeCode(code), measurementType, NormalizeUnit(unit), createdAtUtc);
    }
    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Sensor name cannot be empty.", nameof(name));
        }

        name = name.Trim();

        if (name.Length > 200)
        {
            throw new ArgumentException("Sensor name cannot exceed 200 characters.", nameof(name));
        }

        return name;
    }
    public void UpdateDetails(
        string name,
        MeasurementType measurementType,
        string unit,
        DateTimeOffset updatedAtUtc)
    {
        if (!Enum.IsDefined(typeof(MeasurementType), measurementType))
        {
            throw new ArgumentOutOfRangeException(nameof(measurementType));
        }

        Name = NormalizeName(name);
        Type = measurementType;
        Unit = NormalizeUnit(unit);
        MarkAsUpdated(updatedAtUtc);

    }
    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Sensor code cannot be empty.", nameof(code));
        }

        code = code.Trim();

        if (code.Length > 100)
        {
            throw new ArgumentException("Sensor code cannot exceed 100 characters.", nameof(code));
        }

        return code;
    }
    private static string NormalizeUnit(string unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
        {
            throw new ArgumentException("Sensor unit cannot be empty.", nameof(unit));
        }

        unit = unit.Trim();

        if (unit.Length > 30)
        {
            throw new ArgumentException("Sensor unit cannot exceed 30 characters.", nameof(unit));
        }

        return unit;
    }
}
