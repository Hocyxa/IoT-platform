using IoT.ControlPlane.Domain.Common;

namespace IoT.ControlPlane.Domain.Modules.Device;

public sealed class Device : AuditableEntity
{
    public Guid Id { get; private set; }
    public Guid SiteId { get; private set; }
    public string Name { get; private set; } = null!;
    public string SerialNumber { get; private set; } = null!;
    public DeviceStatus Status { get; private set; }

    private Device()
    {
    }
    private Device(
        Guid id,
        Guid siteId,
        string name,
        string serialNumber,
        DeviceStatus status,
        DateTimeOffset createdAtUtc) : base(createdAtUtc)
    {
        Id = id;
        SiteId = siteId;
        Name = name;
        SerialNumber = serialNumber;
        Status = status;
    }

    public static Device Create(
        Guid id,
        Guid siteId,
        string name,
        string serialNumber,
        DeviceStatus status,
        DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Device ID cannot be empty.", nameof(id));
        }
        if (siteId == Guid.Empty)
        {
            throw new ArgumentException("Site ID cannot be empty.", nameof(siteId));
        }
        return new Device(id, siteId, NormalizeName(name), NormalizeSerialNumber(serialNumber), status, createdAtUtc);

    }
    public void UpdateDetails(
        string name,
        DateTimeOffset updatedAtUtc)
    {
        Name = NormalizeName(name);
        MarkAsUpdated(updatedAtUtc);
    }
    public void ChangeStatus(
        DeviceStatus status,
        DateTimeOffset updatedAtUtc)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        Status = status;
        MarkAsUpdated(updatedAtUtc);
    }
    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Device name cannot be empty.", nameof(name));
        }

        name = name.Trim();

        if (name.Length > 200)
        {
            throw new ArgumentException("Device name cannot exceed 200 characters.", nameof(name));
        }

        return name;
    }
    private static string NormalizeSerialNumber(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            throw new ArgumentException("Serial number cannot be empty.", nameof(serialNumber));
        }

        serialNumber = serialNumber.Trim();

        if (serialNumber.Length > 100)
        {
            throw new ArgumentException("Serial number cannot exceed 100 characters.", nameof(serialNumber));
        }

        return serialNumber;
    }
}
