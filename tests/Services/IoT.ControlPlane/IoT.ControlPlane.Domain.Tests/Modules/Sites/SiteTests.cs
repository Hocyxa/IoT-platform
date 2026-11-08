
using IoT.ControlPlane.Domain.Modules.Sites;

namespace IoT.ControlPlane.Domain.Tests.Modules.Sites;

public sealed class SiteTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_CreatesSite()
    {
        // Arrange
        var id = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        const string name = "Main Office";
        const string address = "Berlin, Germany";

        // Act
        var site = Site.Create(
            id,
            organizationId,
            name,
            address,
            _createdAtUtc);

        // Assert
        Assert.Equal(id, site.Id);
        Assert.Equal(organizationId, site.OrganizationId);
        Assert.Equal(name, site.Name);
        Assert.Equal(address, site.Address);
        Assert.Equal(_createdAtUtc, site.CreatedAtUtc);
        Assert.Equal(_createdAtUtc, site.UpdatedAtUtc);
    }
    [Fact]
    public void Create_WithEmptyOrganizationId_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Site.Create(
                Guid.NewGuid(),
                Guid.Empty,
                "Main Office",
                null,
                _createdAtUtc));
    }

    [Fact]
    public void Create_WithWhitespaceAroundName_TrimsName()
    {
        // Arrange
        const string name = "  Main Office  ";

        // Act
        var site = Site.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            name,
            null,
            _createdAtUtc);

        // Assert
        Assert.Equal("Main Office", site.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespaceName_ThrowsArgumentException(
        string name)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Site.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                name,
                null,
                _createdAtUtc));
    }

    [Fact]
    public void Create_WithNameExceedingMaximumLength_ThrowsArgumentException()
    {
        // Arrange
        var name = new string('a', 201);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Site.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                name,
                null,
                _createdAtUtc));
    }

    [Fact]
    public void Create_WithAddressExceedingMaximumLength_ThrowsArgumentException()
    {
        // Arrange
        var address = new string('a', 501);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Site.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Main Office",
                address,
                _createdAtUtc));
    }

    [Fact]
    public void Create_WithNullAddress_CreatesSiteWithoutAddress()
    {
        // Act
        var site = Site.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Main Office",
            null,
            _createdAtUtc);

        // Assert
        Assert.Null(site.Address);
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesPropertiesAndTimestamp()
    {
        // Arrange
        var site = Site.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Old Name",
            "Old Address",
            _createdAtUtc);

        var updatedAtUtc = _createdAtUtc.AddHours(1);

        // Act
        site.UpdateDetails(
            "New Name",
            "New Address",
            updatedAtUtc);

        // Assert
        Assert.Equal("New Name", site.Name);
        Assert.Equal("New Address", site.Address);
        Assert.Equal(_createdAtUtc, site.CreatedAtUtc);
        Assert.Equal(updatedAtUtc, site.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateDetails_WithNullAddress_ClearsAddress()
    {
        // Arrange
        var site = Site.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Main Office",
            "Berlin, Germany",
            _createdAtUtc);

        // Act
        site.UpdateDetails(
            "Main Office",
            null,
            _createdAtUtc.AddHours(1));

        // Assert
        Assert.Null(site.Address);
    }

    [Fact]
    public void UpdateDetails_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var site = Site.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Main Office",
            null,
            _createdAtUtc);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            site.UpdateDetails(
                " ",
                null,
                _createdAtUtc.AddHours(1)));
    }

    [Fact]
    public void UpdateDetails_WithTimestampEarlierThanCreatedAt_ThrowsArgumentException()
    {
        // Arrange
        var site = Site.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Main Office",
            null,
            _createdAtUtc);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            site.UpdateDetails(
                "New Name",
                null,
                _createdAtUtc.AddMinutes(-1)));
    }


}
