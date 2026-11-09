using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.Commands.CreateDevice;
using IoT.ControlPlane.Application.Modules.Devices.Contracts;
using IoT.ControlPlane.Domain.Modules.Devices;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Devices.Commands.CreateDevice;

public sealed class CreateDeviceCommandHandlerTests
{
    private readonly IDeviceRepository _deviceRepository =
        Substitute.For<IDeviceRepository>();

    private readonly ISiteExistenceChecker _siteExistenceChecker =
        Substitute.For<ISiteExistenceChecker>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    [Fact]
    public async Task Handle_WithExistingSite_CreatesDevice()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        const string name = "Temperature Sensor";
        const string serialNumber = "DEV-001";

        var createdAtUtc = new DateTimeOffset(
            2026, 11, 8, 12, 0, 0, TimeSpan.Zero);

        var cancellationToken = CancellationToken.None;

        var command = new CreateDeviceCommand(
            siteId,
            name,
            serialNumber);

        _siteExistenceChecker
            .ExistsAsync(siteId, cancellationToken)
            .Returns(true);

        _timeProvider
            .GetUtcNow()
            .Returns(createdAtUtc);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        await _deviceRepository.Received(1).AddAsync(
            Arg.Is<Device>(device =>
                device.Id == result &&
                device.SiteId == siteId &&
                device.Name == name &&
                device.SerialNumber == serialNumber &&
                device.Status == DeviceStatus.Active &&
                device.CreatedAtUtc == createdAtUtc),
            cancellationToken);
    }

    [Fact]
    public async Task Handle_WithNonExistingSite_ThrowsKeyNotFoundException()
    {
        // Arrange
        var siteId = Guid.NewGuid();

        var command = new CreateDeviceCommand(
            siteId,
            "Temperature Sensor",
            "DEV-001");

        _siteExistenceChecker
            .ExistsAsync(siteId, Arg.Any<CancellationToken>())
            .Returns(false);

        var handler = CreateHandler();

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Contains(siteId.ToString(), exception.Message);

        await _deviceRepository.DidNotReceive().AddAsync(
            Arg.Any<Device>(),
            Arg.Any<CancellationToken>());

        _timeProvider.DidNotReceive().GetUtcNow();
    }

    [Fact]
    public async Task Handle_WithExistingSite_UsesTimeProvider()
    {
        // Arrange
        var siteId = Guid.NewGuid();

        var createdAtUtc = new DateTimeOffset(
            2026, 11, 8, 15, 30, 0, TimeSpan.Zero);

        var command = new CreateDeviceCommand(
            siteId,
            "Temperature Sensor",
            "DEV-001");

        _siteExistenceChecker
            .ExistsAsync(siteId, Arg.Any<CancellationToken>())
            .Returns(true);

        _timeProvider
            .GetUtcNow()
            .Returns(createdAtUtc);

        var handler = CreateHandler();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _timeProvider.Received(1).GetUtcNow();

        await _deviceRepository.Received(1).AddAsync(
            Arg.Is<Device>(device =>
                device.CreatedAtUtc == createdAtUtc),
            Arg.Any<CancellationToken>());
    }

    private CreateDeviceCommandHandler CreateHandler()
    {
        return new CreateDeviceCommandHandler(
            _deviceRepository,
            _siteExistenceChecker,
            _timeProvider);
    }
}
