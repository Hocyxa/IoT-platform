namespace IoT.ControlPlane.Domain.Common;

public abstract class AuditableEntity
{
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(DateTimeOffset createdAtUtc)
    {
        EnsureUtc(createdAtUtc, nameof(createdAtUtc));

        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    protected void MarkAsUpdated(DateTimeOffset updatedAtUtc)
    {
        EnsureUtc(updatedAtUtc, nameof(updatedAtUtc));
        if (updatedAtUtc < CreatedAtUtc)
        {
            throw new ArgumentException(
                "Update date cannot be earlier than creation date.",
                nameof(updatedAtUtc));
        }
        UpdatedAtUtc = updatedAtUtc;
    }

    private static void EnsureUtc(
        DateTimeOffset value,
        string parameterName)
    {
        if (value.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException(
                "Date must be in UTC.",
                parameterName);
        }
    }
}
