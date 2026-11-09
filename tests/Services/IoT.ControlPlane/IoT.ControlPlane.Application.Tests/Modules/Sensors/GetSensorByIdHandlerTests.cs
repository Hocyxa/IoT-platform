using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.GetSensorById;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Sensors;

public sealed class GetSensorByIdHandlerTests
{
    private readonly ISensorRepository _repository =
        Substitute.For<ISensorRepository>();

    private readonly GetSensorByIdHandler _handler;

    public GetSensorByIdHandlerTests()
    {
        _handler = new GetSensorByIdHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenSensorExists_ReturnsSensorDto()
    {
        var sensor = SensorTestData.CreateSensor();

        _repository.GetByIdAsync(
                sensor.Id,
                Arg.Any<CancellationToken>())
            .Returns(sensor);

        var result = await _handler.Handle(
            new GetSensorByIdQuery(sensor.Id),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(sensor.Id, result.Id);
        Assert.Equal(sensor.DeviceId, result.DeviceId);
        Assert.Equal(sensor.Name, result.Name);
        Assert.Equal(sensor.Code, result.Code);
        Assert.Equal(sensor.Unit, result.Unit);
        Assert.Equal(sensor.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(sensor.UpdatedAtUtc, result.UpdatedAtUtc);
    }

    [Fact]
    public async Task Handle_WhenSensorDoesNotExist_ReturnsNull()
    {
        var id = Guid.NewGuid();

        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns((Sensor?)null);

        var result = await _handler.Handle(
            new GetSensorByIdQuery(id),
            CancellationToken.None);

        Assert.Null(result);
    }
}
