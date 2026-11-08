using IoT.ControlPlane.Domain.Modules.Devices;

namespace IoT.ControlPlane.Domain.Tests.Modules.Devices;

public sealed class DeviceTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_CreatesDevice()
    {
        // Arrange
        var id = Guid.NewGuid();
        var siteId = Guid.NewGuid();
        const string name = "Temperature Sensor";
        const string serialNumber = "DEV-001";

        // Act
        var device = Device.Create(
            id,
            siteId,
            name,
            serialNumber,
            _createdAtUtc);

        // Assert
        Assert.Equal(id, device.Id);
        Assert.Equal(siteId, device.SiteId);
        Assert.Equal(name, device.Name);
        Assert.Equal(serialNumber, device.SerialNumber);
        Assert.Equal(DeviceStatus.Active, device.Status);
        Assert.Equal(_createdAtUtc, device.CreatedAtUtc);
        Assert.Equal(_createdAtUtc, device.UpdatedAtUtc);
    }

    [Fact]
    public void Create_WithEmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Device.Create(
                Guid.Empty,
                Guid.NewGuid(),
                "Device",
                "DEV-001",
                _createdAtUtc));
    }

    [Fact]
    public void Create_WithEmptySiteId_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Device.Create(
                Guid.NewGuid(),
                Guid.Empty,
                "Device",
                "DEV-001",
                _createdAtUtc));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespaceName_ThrowsArgumentException(
        string name)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Device.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                name,
                "DEV-001",
                _createdAtUtc));
    }

    [Fact]
    public void Create_WithNameExceedingMaximumLength_ThrowsArgumentException()
    {
        // Arrange
        var name = new string('a', 201);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Device.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                name,
                "DEV-001",
                _createdAtUtc));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespaceSerialNumber_ThrowsArgumentException(
        string serialNumber)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Device.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Device",
                serialNumber,
                _createdAtUtc));
    }

    [Fact]
    public void Create_WithSerialNumberExceedingMaximumLength_ThrowsArgumentException()
    {
        // Arrange
        var serialNumber = new string('a', 101);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Device.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Device",
                serialNumber,
                _createdAtUtc));
    }

    [Fact]
    public void Create_WithWhitespaceAroundValues_TrimsValues()
    {
        // Act
        var device = Device.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "  Device  ",
            "  DEV-001  ",
            _createdAtUtc);

        // Assert
        Assert.Equal("Device", device.Name);
        Assert.Equal("DEV-001", device.SerialNumber);
    }

    [Fact]
    public void UpdateDetails_WithValidName_UpdatesNameAndTimestamp()
    {
        // Arrange
        var device = CreateDevice();
        var updatedAtUtc = _createdAtUtc.AddHours(1);

        // Act
        device.UpdateDetails("Updated Device", updatedAtUtc);

        // Assert
        Assert.Equal("Updated Device", device.Name);
        Assert.Equal("DEV-001", device.SerialNumber);
        Assert.Equal(_createdAtUtc, device.CreatedAtUtc);
        Assert.Equal(updatedAtUtc, device.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateDetails_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var device = CreateDevice();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            device.UpdateDetails(" ", _createdAtUtc.AddHours(1)));
    }

    [Fact]
    public void ChangeStatus_WithValidStatus_UpdatesStatusAndTimestamp()
    {
        // Arrange
        var device = CreateDevice();
        var updatedAtUtc = _createdAtUtc.AddHours(1);

        // Act
        device.ChangeStatus(DeviceStatus.Maintenance, updatedAtUtc);

        // Assert
        Assert.Equal(DeviceStatus.Maintenance, device.Status);
        Assert.Equal(updatedAtUtc, device.UpdatedAtUtc);
    }

    [Theory]
    [InlineData(DeviceStatus.Active)]
    [InlineData(DeviceStatus.Maintenance)]
    [InlineData(DeviceStatus.Disabled)]
    public void ChangeStatus_WithDefinedStatus_UpdatesStatus(
        DeviceStatus status)
    {
        // Arrange
        var device = CreateDevice();

        // Act
        device.ChangeStatus(status, _createdAtUtc.AddHours(1));

        // Assert
        Assert.Equal(status, device.Status);
    }

    [Fact]
    public void ChangeStatus_WithUndefinedStatus_ThrowsArgumentException()
    {
        // Arrange
        var device = CreateDevice();
        var invalidStatus = (DeviceStatus)999;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            device.ChangeStatus(invalidStatus, _createdAtUtc.AddHours(1)));
    }

    [Fact]
    public void ChangeStatus_WithTimestampEarlierThanCreatedAt_ThrowsArgumentException()
    {
        // Arrange
        var device = CreateDevice();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            device.ChangeStatus(
                DeviceStatus.Disabled,
                _createdAtUtc.AddMinutes(-1)));
    }

    private static Device CreateDevice()
    {
        return Device.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Device",
            "DEV-001",
            _createdAtUtc);
    }
}
