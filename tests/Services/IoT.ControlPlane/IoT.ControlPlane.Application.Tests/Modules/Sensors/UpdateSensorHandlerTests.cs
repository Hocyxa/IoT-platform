using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.UpdateSensor;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Sensors;

public sealed class UpdateSensorHandlerTests
{
    private readonly ISensorRepository _repository =
        Substitute.For<ISensorRepository>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    private readonly UpdateSensorHandler _handler;

    public UpdateSensorHandlerTests()
    {
        _handler = new UpdateSensorHandler(
            _repository,
            _timeProvider);
    }

    [Fact]
    public async Task Handle_WhenSensorExists_UpdatesSensor()
    {
        var sensor = SensorTestData.CreateSensor();

        var updatedAtUtc = SensorTestData.CreatedAtUtc.AddHours(1);

        var command = new UpdateSensorCommand(
            sensor.Id,
            "Updated sensor",
            MeasurementType.Humidity,
            "%");

        _repository.GetByIdAsync(
                sensor.Id,
                Arg.Any<CancellationToken>())
            .Returns(sensor);

        _timeProvider.GetUtcNow().Returns(updatedAtUtc);

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        Assert.True(result);
        Assert.Equal("Updated sensor", sensor.Name);
        Assert.Equal("%", sensor.Unit);
        Assert.Equal(updatedAtUtc, sensor.UpdatedAtUtc);

        await _repository.Received(1).UpdateAsync(
            sensor,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSensorDoesNotExist_ReturnsFalse()
    {
        var id = Guid.NewGuid();

        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns((Sensor?)null);

        var command = new UpdateSensorCommand(
            id,
            "Updated sensor",
            MeasurementType.Humidity,
            "%");

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        Assert.False(result);

        await _repository.DidNotReceive().UpdateAsync(
            Arg.Any<Sensor>(),
            Arg.Any<CancellationToken>());
    }
}
