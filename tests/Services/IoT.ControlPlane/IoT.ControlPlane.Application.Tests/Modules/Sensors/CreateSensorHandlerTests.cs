using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.Contracts;
using IoT.ControlPlane.Application.Modules.Sensors.CreateSensor;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Sensors;

public sealed class CreateSensorHandlerTests
{
    private readonly ISensorRepository _repository =
        Substitute.For<ISensorRepository>();

    private readonly ISensorDeviceExistenceChecker _deviceChecker =
        Substitute.For<ISensorDeviceExistenceChecker>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    private readonly CreateSensorHandler _handler;

    public CreateSensorHandlerTests()
    {
        _handler = new CreateSensorHandler(
            _repository,
            _deviceChecker,
            _timeProvider);
    }

    [Fact]
    public async Task Handle_WhenDeviceExists_CreatesSensor()
    {
        var deviceId = Guid.NewGuid();
        var updatedAtUtc = SensorTestData.CreatedAtUtc.AddHours(1);

        var command = new CreateSensorCommand(
            deviceId,
            "Temperature sensor",
            "TEMP-01",
            MeasurementType.Temperature,
            "°C");

        _deviceChecker.ExistsAsync(deviceId, Arg.Any<CancellationToken>())
            .Returns(true);

        _timeProvider.GetUtcNow().Returns(updatedAtUtc);

        Sensor? savedSensor = null;

        _repository.AddAsync(
                Arg.Do<Sensor>(sensor => savedSensor = sensor),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result);
        Assert.NotNull(savedSensor);
        Assert.Equal(result, savedSensor.Id);
        Assert.Equal(deviceId, savedSensor.DeviceId);
        Assert.Equal("Temperature sensor", savedSensor.Name);
        Assert.Equal("TEMP-01", savedSensor.Code);
        Assert.Equal("°C", savedSensor.Unit);
        Assert.Equal(updatedAtUtc, savedSensor.CreatedAtUtc);

        await _deviceChecker.Received(1).ExistsAsync(
            deviceId,
            Arg.Any<CancellationToken>());

        await _repository.Received(1).AddAsync(
            Arg.Any<Sensor>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDeviceDoesNotExist_ThrowsKeyNotFoundException()
    {
        var deviceId = Guid.NewGuid();

        var command = new CreateSensorCommand(
            deviceId,
            "Temperature sensor",
            "TEMP-01",
            MeasurementType.Temperature,
            "°C");

        _deviceChecker.ExistsAsync(deviceId, Arg.Any<CancellationToken>())
            .Returns(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        await _repository.DidNotReceive().AddAsync(
            Arg.Any<Sensor>(),
            Arg.Any<CancellationToken>());
    }
}
