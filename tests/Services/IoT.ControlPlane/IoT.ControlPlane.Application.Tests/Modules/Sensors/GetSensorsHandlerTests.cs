using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.GetSensors;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Sensors;

public sealed class GetSensorsHandlerTests
{
    private readonly ISensorRepository _repository =
        Substitute.For<ISensorRepository>();

    private readonly GetSensorsHandler _handler;

    public GetSensorsHandlerTests()
    {
        _handler = new GetSensorsHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenSensorsExist_ReturnsSensorDtos()
    {
        var sensors = new[]
        {
            SensorTestData.CreateSensor(name: "Temperature"),
            SensorTestData.CreateSensor(name: "Humidity")
        };

        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<Sensor>)sensors);

        var result = await _handler.Handle(
            new GetSensorsQuery(),
            CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Temperature", result[0].Name);
        Assert.Equal("Humidity", result[1].Name);
    }

    [Fact]
    public async Task Handle_WhenNoSensorsExist_ReturnsEmptyList()
    {
        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Sensor>());

        var result = await _handler.Handle(
            new GetSensorsQuery(),
            CancellationToken.None);

        Assert.Empty(result);
    }
}
