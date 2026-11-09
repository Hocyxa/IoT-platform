using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.DeleteSensor;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Sensors;

public sealed class DeleteSensorHandlerTests
{
    private readonly ISensorRepository _repository =
        Substitute.For<ISensorRepository>();

    private readonly DeleteSensorHandler _handler;

    public DeleteSensorHandlerTests()
    {
        _handler = new DeleteSensorHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenSensorExists_DeletesSensor()
    {
        var sensor = SensorTestData.CreateSensor();

        _repository.GetByIdAsync(
                sensor.Id,
                Arg.Any<CancellationToken>())
            .Returns(sensor);

        var result = await _handler.Handle(
            new DeleteSensorCommand(sensor.Id),
            CancellationToken.None);

        Assert.True(result);

        await _repository.Received(1).DeleteAsync(
            sensor,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSensorDoesNotExist_ReturnsFalse()
    {
        var id = Guid.NewGuid();

        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns((Sensor?)null);

        var result = await _handler.Handle(
            new DeleteSensorCommand(id),
            CancellationToken.None);

        Assert.False(result);

        await _repository.DidNotReceive().DeleteAsync(
            Arg.Any<Sensor>(),
            Arg.Any<CancellationToken>());
    }
}
