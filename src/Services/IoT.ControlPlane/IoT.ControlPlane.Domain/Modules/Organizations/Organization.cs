using IoT.ControlPlane.Domain.Common;

namespace IoT.ControlPlane.Domain.Modules.Organizations;

public sealed class Organization : AuditableEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private Organization()
    {
    }
    private Organization(
        Guid id,
        string name,
        DateTimeOffset createdAtUtc) : base(createdAtUtc)
    {
        Id = id;
        Name = name;
    }
    private static string NormalizeName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        var normalizedName = name.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException(
                "Organization name cannot be empty.",
                nameof(name));
        }
        if (normalizedName.Length > 200)
        {
            throw new ArgumentException(
                "Organization name cannot exceed 200 characters.",
                nameof(name));
        }
        return normalizedName;
    }

    public static Organization Create(Guid id, string name, DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Organization ID cannot be empty.", nameof(id));
        }
        var normalizedName = NormalizeName(name);
        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("Creation date must be in UTC.",
                nameof(createdAtUtc));
        }

        return new Organization(id, normalizedName, createdAtUtc);
    }

    public void Rename(string name, DateTimeOffset updatedAtUtc)
    {
        Name = NormalizeName(name);
        MarkAsUpdated(updatedAtUtc);
    }


}
