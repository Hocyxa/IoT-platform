using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.GetSensorsByDevice;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Sensors;

public sealed class GetSensorsByDeviceHandlerTests
{
    private readonly ISensorRepository _repository =
        Substitute.For<ISensorRepository>();

    private readonly GetSensorsByDeviceHandler _handler;

    public GetSensorsByDeviceHandlerTests()
    {
        _handler = new GetSensorsByDeviceHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenSensorsExist_ReturnsSensorsForDevice()
    {
        var deviceId = Guid.NewGuid();

        var sensors = new[]
        {
            SensorTestData.CreateSensor(deviceId: deviceId, name: "Temperature"),
            SensorTestData.CreateSensor(deviceId: deviceId, name: "Humidity")
        };

        _repository.GetByDeviceIdAsync(
                deviceId,
                Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Sensor>)sensors);

        var result = await _handler.Handle(
            new GetSensorsByDeviceQuery(deviceId),
            CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result, sensor => Assert.Equal(deviceId, sensor.DeviceId));
        Assert.Equal("Temperature", result[0].Name);
        Assert.Equal("Humidity", result[1].Name);

        await _repository.Received(1).GetByDeviceIdAsync(
            deviceId,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoSensorsExist_ReturnsEmptyList()
    {
        var deviceId = Guid.NewGuid();

        _repository.GetByDeviceIdAsync(
                deviceId,
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Sensor>());

        var result = await _handler.Handle(
            new GetSensorsByDeviceQuery(deviceId),
            CancellationToken.None);

        Assert.Empty(result);
    }
}
