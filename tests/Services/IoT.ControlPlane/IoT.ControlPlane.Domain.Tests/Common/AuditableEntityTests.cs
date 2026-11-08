using IoT.ControlPlane.Domain.Common;

namespace IoT.ControlPlane.Domain.Tests.Common;

public sealed class AuditableEntityTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_WithValidUtcTimestamp_SetsCreatedAndUpdatedAt()
    {
        // Arrange & Act
        var entity = new TestAuditableEntity(_createdAtUtc);

        // Assert
        Assert.Equal(_createdAtUtc, entity.CreatedAtUtc);
        Assert.Equal(_createdAtUtc, entity.UpdatedAtUtc);
    }

    [Fact]
    public void Constructor_WithNonUtcTimestamp_ThrowsArgumentException()
    {
        // Arrange
        var nonUtcTimestamp = new DateTimeOffset(
            2026, 11, 8, 15, 0, 0,
            TimeSpan.FromHours(3));

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new TestAuditableEntity(nonUtcTimestamp));
    }

    [Fact]
    public void MarkAsUpdated_WithValidTimestamp_UpdatesUpdatedAt()
    {
        // Arrange
        var entity = new TestAuditableEntity(_createdAtUtc);
        var updatedAtUtc = _createdAtUtc.AddHours(1);

        // Act
        entity.Update(updatedAtUtc);

        // Assert
        Assert.Equal(_createdAtUtc, entity.CreatedAtUtc);
        Assert.Equal(updatedAtUtc, entity.UpdatedAtUtc);
    }

    [Fact]
    public void MarkAsUpdated_WithTimestampEarlierThanCreatedAt_ThrowsArgumentException()
    {
        // Arrange
        var entity = new TestAuditableEntity(_createdAtUtc);
        var earlierTimestamp = _createdAtUtc.AddMinutes(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            entity.Update(earlierTimestamp));
    }

    [Fact]
    public void MarkAsUpdated_WithNonUtcTimestamp_ThrowsArgumentException()
    {
        // Arrange
        var entity = new TestAuditableEntity(_createdAtUtc);

        var nonUtcTimestamp = new DateTimeOffset(
            2026, 11, 8, 15, 0, 0,
            TimeSpan.FromHours(3));

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            entity.Update(nonUtcTimestamp));
    }

    private sealed class TestAuditableEntity : AuditableEntity
    {
        public TestAuditableEntity(DateTimeOffset createdAtUtc)
            : base(createdAtUtc)
        {
        }

        public void Update(DateTimeOffset updatedAtUtc)
        {
            MarkAsUpdated(updatedAtUtc);
        }
    }
}
