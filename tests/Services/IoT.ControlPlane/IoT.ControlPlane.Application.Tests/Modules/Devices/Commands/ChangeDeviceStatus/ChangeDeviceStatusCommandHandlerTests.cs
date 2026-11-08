using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.Commands.ChangeDeviceStatus;
using IoT.ControlPlane.Domain.Modules.Devices;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Devices.Commands.ChangeDeviceStatus;

public sealed class ChangeDeviceStatusCommandHandlerTests
{
    private readonly IDeviceRepository _deviceRepository =
        Substitute.For<IDeviceRepository>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    private static readonly DateTimeOffset _createdAtUtc =
        new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Handle_WithExistingDevice_ChangesStatus()
    {
        // Arrange
        var device = CreateDevice();
        var updatedAtUtc = _createdAtUtc.AddHours(1);
        var newStatus = DeviceStatus.Maintenance;

        var command = new ChangeDeviceStatusCommand(
            device.Id,
            newStatus);

        _deviceRepository
            .GetByIdAsync(device.Id, Arg.Any<CancellationToken>())
            .Returns(device);

        _timeProvider
            .GetUtcNow()
            .Returns(updatedAtUtc);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(newStatus, device.Status);
        Assert.Equal(updatedAtUtc, device.UpdatedAtUtc);

        await _deviceRepository.Received(1).UpdateAsync(
            device,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistingDevice_ReturnsFalse()
    {
        // Arrange
        var deviceId = Guid.NewGuid();

        var command = new ChangeDeviceStatusCommand(
            deviceId,
            DeviceStatus.Disabled);

        _deviceRepository
            .GetByIdAsync(deviceId, Arg.Any<CancellationToken>())
            .Returns((Device?)null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.False(result);

        await _deviceRepository.DidNotReceive().UpdateAsync(
            Arg.Any<Device>(),
            Arg.Any<CancellationToken>());

        _timeProvider.DidNotReceive().GetUtcNow();
    }

    [Theory]
    [InlineData(DeviceStatus.Active)]
    [InlineData(DeviceStatus.Maintenance)]
    [InlineData(DeviceStatus.Disabled)]
    public async Task Handle_WithDefinedStatus_UpdatesDeviceStatus(
        DeviceStatus status)
    {
        // Arrange
        var device = CreateDevice();
        var updatedAtUtc = _createdAtUtc.AddHours(1);

        _deviceRepository
            .GetByIdAsync(device.Id, Arg.Any<CancellationToken>())
            .Returns(device);

        _timeProvider
            .GetUtcNow()
            .Returns(updatedAtUtc);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new ChangeDeviceStatusCommand(device.Id, status),
            CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(status, device.Status);
    }

    [Fact]
    public async Task Handle_WithExistingDevice_UsesTimeProvider()
    {
        // Arrange
        var device = CreateDevice();
        var updatedAtUtc = _createdAtUtc.AddHours(2);

        _deviceRepository
            .GetByIdAsync(device.Id, Arg.Any<CancellationToken>())
            .Returns(device);

        _timeProvider
            .GetUtcNow()
            .Returns(updatedAtUtc);

        var handler = CreateHandler();

        // Act
        await handler.Handle(
            new ChangeDeviceStatusCommand(
                device.Id,
                DeviceStatus.Disabled),
            CancellationToken.None);

        // Assert
        _timeProvider.Received(1).GetUtcNow();

        Assert.Equal(updatedAtUtc, device.UpdatedAtUtc);
    }

    [Fact]
    public async Task Handle_WithUndefinedStatus_ThrowsArgumentException()
    {
        // Arrange
        var device = CreateDevice();
        var invalidStatus = (DeviceStatus)999;

        _deviceRepository
            .GetByIdAsync(device.Id, Arg.Any<CancellationToken>())
            .Returns(device);

        _timeProvider
            .GetUtcNow()
            .Returns(_createdAtUtc.AddHours(1));

        var handler = CreateHandler();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            handler.Handle(
                new ChangeDeviceStatusCommand(device.Id, invalidStatus),
                CancellationToken.None));

        await _deviceRepository.DidNotReceive().UpdateAsync(
            Arg.Any<Device>(),
            Arg.Any<CancellationToken>());
    }

    private ChangeDeviceStatusCommandHandler CreateHandler()
    {
        return new ChangeDeviceStatusCommandHandler(
            _deviceRepository,
            _timeProvider);
    }

    private static Device CreateDevice()
    {
        return Device.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Temperature Sensor",
            "DEV-001",
            _createdAtUtc);
    }
}
