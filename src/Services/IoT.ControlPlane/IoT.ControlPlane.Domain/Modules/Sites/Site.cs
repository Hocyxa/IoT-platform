using IoT.ControlPlane.Domain.Common;
namespace IoT.ControlPlane.Domain.Modules.Sites;

public sealed class Site : AuditableEntity
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }
    private Site()
    {
    }

    private Site(
        Guid id,
        Guid organizationId,
        string name,
        string? address,
        DateTimeOffset createdAtUtc)
        : base(createdAtUtc)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        Address = address;

    }
    public static Site Create(
        Guid id,
        Guid organizationId,
        string name,
        string? address,
        DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Site ID cannot be empty.",
                nameof(id));
        }

        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID cannot be empty.",
                nameof(organizationId));
        }

        var normalizedName = NormalizeName(name);
        var normalizedAddress = NormalizeAddress(address);

        if (createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException(
                "Creation date must be in UTC.",
                nameof(createdAtUtc));
        }

        return new Site(
            id,
            organizationId,
            normalizedName,
            normalizedAddress,
            createdAtUtc);
    }

    public void UpdateDetails(string name, string? address, DateTimeOffset updatedAtUtc)
    {
        Name = NormalizeName(name);
        Address = NormalizeAddress(address);
        MarkAsUpdated(updatedAtUtc);
    }

    private static string NormalizeName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        var normalizedName = name.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException(
                "Site name cannot be empty.",
                nameof(name));
        }

        if (normalizedName.Length > 200)
        {
            throw new ArgumentException(
                "Site name cannot exceed 200 characters.",
                nameof(name));
        }

        return normalizedName;
    }

    private static string? NormalizeAddress(string? address)
    {
        if (address is null)
        {
            return null;
        }

        var normalizedAddress = address.Trim();

        if (normalizedAddress.Length > 500)
        {
            throw new ArgumentException(
                "Site address cannot exceed 500 characters.",
                nameof(address));
        }

        return string.IsNullOrWhiteSpace(normalizedAddress)
            ? null
            : normalizedAddress;
    }
}
