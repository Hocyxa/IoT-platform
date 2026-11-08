using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.Queries.GetDevicesBySite;
using IoT.ControlPlane.Domain.Modules.Devices;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Devices.Queries;

public sealed class GetDevicesBySiteQueryHandlerTests
{
    private readonly IDeviceRepository _repository =
        Substitute.For<IDeviceRepository>();

    private GetDevicesBySiteQueryHandler CreateHandler()
    {
        return new GetDevicesBySiteQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenDevicesExist_ReturnsDeviceDtos()
    {
        // Arrange
        var siteId = Guid.NewGuid();

        var devices = new List<Device>
        {
            CreateDevice(siteId, "Device 1", "SN-001"),
            CreateDevice(siteId, "Device 2", "SN-002"),
        };

        _repository
            .GetBySiteIdAsync(siteId, Arg.Any<CancellationToken>())
            .Returns(devices);

        var handler = CreateHandler();
        var query = new GetDevicesBySiteQuery(siteId);

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
            .GetBySiteIdAsync(siteId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoDevicesExist_ReturnsEmptyList()
    {
        // Arrange
        var siteId = Guid.NewGuid();

        _repository
            .GetBySiteIdAsync(siteId, Arg.Any<CancellationToken>())
            .Returns(new List<Device>());

        var handler = CreateHandler();
        var query = new GetDevicesBySiteQuery(siteId);

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Empty(result);

        await _repository.Received(1)
            .GetBySiteIdAsync(siteId, Arg.Any<CancellationToken>());
    }

    private static Device CreateDevice(
        Guid siteId,
        string name,
        string serialNumber)
    {
        return Device.Create(
            Guid.NewGuid(),
            siteId,
            name,
            serialNumber,
            DateTimeOffset.UtcNow);
    }
}
