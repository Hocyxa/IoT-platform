using IoT.ControlPlane.Domain.Modules.Organizations;

namespace IoT.ControlPlane.Domain.Tests.Modules.Organizations;

public sealed class OrganizationTests
{
    private static readonly DateTimeOffset _createdAtUtc = new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_CteatesOrganization()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string name = "Test Organization";

        // Act
        var organization = Organization.Create(id, name, _createdAtUtc);

        // Assert
        Assert.Equal(id, organization.Id);
        Assert.Equal(name, organization.Name);
        Assert.Equal(_createdAtUtc, organization.CreatedAtUtc);
        Assert.Equal(_createdAtUtc, organization.UpdatedAtUtc);
    }

    [Fact]
    public void Create_WithWhitespaceAroundName_TrimsName()
    {
        // Arrange
        const string name = "  Test Organization  ";

        // Act
        var organization = Organization.Create(
            Guid.NewGuid(),
            name,
            _createdAtUtc);

        // Assert
        Assert.Equal("Test Organization", organization.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespaceName_ThrowsArgumentException(
        string name)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Organization.Create(
                Guid.NewGuid(),
                name,
                _createdAtUtc));
    }

    [Fact]
    public void Create_WithNameExceedingMaximumLength_ThrowsArgumentException()
    {
        // Arrange
        var name = new string('a', 201);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Organization.Create(
                Guid.NewGuid(),
                name,
                _createdAtUtc));
    }

    [Fact]
    public void Rename_WithValidName_UpdatesNameAndTimestamp()
    {
        // Arrange
        var organization = Organization.Create(
            Guid.NewGuid(),
            "Old Name",
            _createdAtUtc);

        var updatedAtUtc = _createdAtUtc.AddHours(1);

        // Act
        organization.Rename("New Name", updatedAtUtc);

        // Assert
        Assert.Equal("New Name", organization.Name);
        Assert.Equal(_createdAtUtc, organization.CreatedAtUtc);
        Assert.Equal(updatedAtUtc, organization.UpdatedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Rename_WithEmptyOrWhitespaceName_ThrowsArgumentException(string name)
    {
        // Arrange
        var organization = Organization.Create(
            Guid.NewGuid(),
            "Test Organization",
            _createdAtUtc);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            organization.Rename(name, _createdAtUtc.AddHours(1)));
    }
    [Fact]
    public void Rename_WithTimestampEarlierThanCreatedAt_ThrowsArgumentException()
    {
        // Arrange
        var organization = Organization.Create(
            Guid.NewGuid(),
            "Test Organization",
            _createdAtUtc);

        var earlierTimestamp = _createdAtUtc.AddMinutes(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            organization.Rename("New Name", earlierTimestamp));
    }
}
