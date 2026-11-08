using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.Queries.GetDevices;
using IoT.ControlPlane.Domain.Modules.Devices;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Devices;

public sealed class GetDevicesQueryHandlerTests
{
    private readonly IDeviceRepository _repository =
        Substitute.For<IDeviceRepository>();

    private GetDevicesQueryHandler CreateHandler()
    {
        return new GetDevicesQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenDevicesExist_ReturnsDeviceDtos()
    {
        // Arrange
        var devices = new List<Device>
        {
            CreateDevice("Device 1", "SN-001"),
            CreateDevice("Device 2", "SN-002"),
        };

        _repository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(devices);

        var handler = CreateHandler();
        var query = new GetDevicesQuery();

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Equal(devices.Count, result.Count);

        for (var i = 0; i < devices.Count; i++)
        {
            Assert.Equal(devices[i].Id, result[i].Id);
            Assert.Equal(devices[i].SiteId, result[i].SiteId);
            Assert.Equal(devices[i].Name, result[i].Name);
            Assert.Equal(devices[i].SerialNumber, result[i].SerialNumber);
            Assert.Equal(devices[i].Status, result[i].Status);
        }

        await _repository.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoDevicesExist_ReturnsEmptyList()
    {
        // Arrange
        _repository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Device>());

        var handler = CreateHandler();
        var query = new GetDevicesQuery();

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Empty(result);

        await _repository.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    private static Device CreateDevice(string name, string serialNumber)
    {
        return Device.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            name,
            serialNumber,
            DateTimeOffset.UtcNow);
    }
}
