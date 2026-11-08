using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.Commands.UpdateDevice;
using IoT.ControlPlane.Domain.Modules.Devices;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Devices.Commands.UpdateDevice;

public sealed class UpdateDeviceCommandHandlerTests
{
    private readonly IDeviceRepository _deviceRepository =
        Substitute.For<IDeviceRepository>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    [Fact]
    public async Task Handle_WithExistingDevice_UpdatesDevice()
    {
        // Arrange
        var device = CreateDevice();
        var updatedAtUtc = CreatedAtUtc.AddHours(1);
        const string newName = "Updated Device";

        var command = new UpdateDeviceCommand(
            device.Id,
            newName);

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

        Assert.Equal(newName, device.Name);
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

        var command = new UpdateDeviceCommand(
            deviceId,
            "Updated Device");

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

    [Fact]
    public async Task Handle_WithExistingDevice_UsesTimeProvider()
    {
        // Arrange
        var device = CreateDevice();
        var updatedAtUtc = CreatedAtUtc.AddHours(2);

        _deviceRepository
            .GetByIdAsync(device.Id, Arg.Any<CancellationToken>())
            .Returns(device);

        _timeProvider
            .GetUtcNow()
            .Returns(updatedAtUtc);

        var handler = CreateHandler();

        // Act
        await handler.Handle(
            new UpdateDeviceCommand(device.Id, "New Name"),
            CancellationToken.None);

        // Assert
        _timeProvider.Received(1).GetUtcNow();

        Assert.Equal(updatedAtUtc, device.UpdatedAtUtc);
    }

    private UpdateDeviceCommandHandler CreateHandler()
    {
        return new UpdateDeviceCommandHandler(
            _deviceRepository,
            _timeProvider);
    }

    private static Device CreateDevice()
    {
        return Device.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Original Device",
            "DEV-001",
            CreatedAtUtc);
    }

    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);
}
