using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.Commands.DeleteDevice;
using IoT.ControlPlane.Domain.Modules.Devices;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Devices.Commands.DeleteDevice;

public sealed class DeleteDeviceCommandHandlerTests
{
    private readonly IDeviceRepository _deviceRepository =
        Substitute.For<IDeviceRepository>();

    [Fact]
    public async Task Handle_WithExistingDevice_DeletesDevice()
    {
        // Arrange
        var device = CreateDevice();

        _deviceRepository
            .GetByIdAsync(device.Id, Arg.Any<CancellationToken>())
            .Returns(device);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new DeleteDeviceCommand(device.Id),
            CancellationToken.None);

        // Assert
        Assert.True(result);

        await _deviceRepository.Received(1).DeleteAsync(
            device,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistingDevice_ReturnsFalse()
    {
        // Arrange
        var deviceId = Guid.NewGuid();

        _deviceRepository
            .GetByIdAsync(deviceId, Arg.Any<CancellationToken>())
            .Returns((Device?)null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new DeleteDeviceCommand(deviceId),
            CancellationToken.None);

        // Assert
        Assert.False(result);

        await _deviceRepository.DidNotReceive().DeleteAsync(
            Arg.Any<Device>(),
            Arg.Any<CancellationToken>());
    }

    private DeleteDeviceCommandHandler CreateHandler()
    {
        return new DeleteDeviceCommandHandler(_deviceRepository);
    }

    private static Device CreateDevice()
    {
        return Device.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Temperature Sensor",
            "DEV-001",
            new DateTimeOffset(
                2026, 11, 8, 12, 0, 0, TimeSpan.Zero));
    }
}
