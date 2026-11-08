using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.Queries.GetDeviceById;
using IoT.ControlPlane.Domain.Modules.Devices;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Devices;

public sealed class GetDeviceByIdQueryHandlerTests
{
    private readonly IDeviceRepository _repository =
        Substitute.For<IDeviceRepository>();

    private GetDeviceByIdQueryHandler CreateHandler()
    {
        return new GetDeviceByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenDeviceExists_ReturnsDeviceDto()
    {
        // Arrange
        var device = CreateDevice();
        _repository
            .GetByIdAsync(device.Id, Arg.Any<CancellationToken>())
            .Returns(device);

        var handler = CreateHandler();
        var query = new GetDeviceByIdQuery(device.Id);

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(device.Id, result.Id);
        Assert.Equal(device.SiteId, result.SiteId);
        Assert.Equal(device.Name, result.Name);
        Assert.Equal(device.SerialNumber, result.SerialNumber);
        Assert.Equal(device.Status, result.Status);
        Assert.Equal(device.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(device.UpdatedAtUtc, result.UpdatedAtUtc);

        await _repository.Received(1)
            .GetByIdAsync(device.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDeviceDoesNotExist_ReturnsNull()
    {
        // Arrange
        var deviceId = Guid.NewGuid();

        _repository
            .GetByIdAsync(deviceId, Arg.Any<CancellationToken>())
            .Returns((Device?)null);

        var handler = CreateHandler();
        var query = new GetDeviceByIdQuery(deviceId);

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Null(result);

        await _repository.Received(1)
            .GetByIdAsync(deviceId, Arg.Any<CancellationToken>());
    }

    private static Device CreateDevice()
    {
        return Device.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test device",
            "SN-12345",
            DateTimeOffset.UtcNow);
    }
}
