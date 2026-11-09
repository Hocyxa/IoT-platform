
using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Domain.Tests.Modules.Sensors;

public sealed class SensorTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new(2026, 11, 9, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_CreatesSensor()
    {
        // Arrange
        var id = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        // Act
        var sensor = Sensor.Create(
            id,
            deviceId,
            "Temperature sensor",
            "TEMP-01",
            MeasurementType.Temperature,
            "°C",
            _createdAtUtc);

        // Assert
        Assert.Equal(id, sensor.Id);
        Assert.Equal(deviceId, sensor.DeviceId);
        Assert.Equal("Temperature sensor", sensor.Name);
        Assert.Equal("TEMP-01", sensor.Code);
        Assert.Equal(MeasurementType.Temperature, sensor.MeasurementType);
        Assert.Equal("°C", sensor.Unit);
        Assert.Equal(_createdAtUtc, sensor.CreatedAtUtc);
        Assert.Equal(_createdAtUtc, sensor.UpdatedAtUtc);
    }

    [Fact]
    public void Create_WithEmptyId_ThrowsArgumentException()
    {
        // Act
        var act = () => Sensor.Create(
            Guid.Empty,
            Guid.NewGuid(),
            "Sensor",
            "TEMP-01",
            MeasurementType.Temperature,
            "°C",
            _createdAtUtc);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithEmptyDeviceId_ThrowsArgumentException()
    {
        // Act
        var act = () => Sensor.Create(
            Guid.NewGuid(),
            Guid.Empty,
            "Sensor",
            "TEMP-01",
            MeasurementType.Temperature,
            "°C",
            _createdAtUtc);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ThrowsArgumentException(string name)
    {
        // Act
        var act = () => Sensor.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            name,
            "TEMP-01",
            MeasurementType.Temperature,
            "°C",
            _createdAtUtc);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithNameExceedingMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var name = new string('A', 201);

        // Act
        var act = () => Sensor.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            name,
            "TEMP-01",
            MeasurementType.Temperature,
            "°C",
            _createdAtUtc);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithEmptyCode_ThrowsArgumentException(string code)
    {
        // Act
        var act = () => Sensor.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sensor",
            code,
            MeasurementType.Temperature,
            "°C",
            _createdAtUtc);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithCodeExceedingMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var code = new string('A', 101);

        // Act
        var act = () => Sensor.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sensor",
            code,
            MeasurementType.Temperature,
            "°C",
            _createdAtUtc);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithEmptyUnit_ThrowsArgumentException(string unit)
    {
        // Act
        var act = () => Sensor.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sensor",
            "TEMP-01",
            MeasurementType.Temperature,
            unit,
            _createdAtUtc);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Create_WithUnitExceedingMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var unit = new string('A', 31);

        // Act
        var act = () => Sensor.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sensor",
            "TEMP-01",
            MeasurementType.Temperature,
            unit,
            _createdAtUtc);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData(MeasurementType.Temperature)]
    [InlineData(MeasurementType.Humidity)]
    [InlineData(MeasurementType.Pressure)]
    [InlineData(MeasurementType.Voltage)]
    [InlineData(MeasurementType.Current)]
    [InlineData(MeasurementType.Power)]
    [InlineData(MeasurementType.Frequency)]
    [InlineData(MeasurementType.Speed)]
    [InlineData(MeasurementType.Vibration)]
    [InlineData(MeasurementType.Level)]
    public void Create_WithDefinedMeasurementType_CreatesSensor(
        MeasurementType measurementType)
    {
        // Act
        var sensor = Sensor.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sensor",
            "SENSOR-01",
            measurementType,
            "unit",
            _createdAtUtc);

        // Assert
        Assert.Equal(measurementType, sensor.MeasurementType);
    }

    [Fact]
    public void Create_WithUndefinedMeasurementType_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var measurementType = (MeasurementType)999;

        // Act
        var act = () => Sensor.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sensor",
            "SENSOR-01",
            measurementType,
            "unit",
            _createdAtUtc);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Create_TrimsStringProperties()
    {
        // Act
        var sensor = Sensor.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "  Temperature sensor  ",
            "  TEMP-01  ",
            MeasurementType.Temperature,
            "  °C  ",
            _createdAtUtc);

        // Assert
        Assert.Equal("Temperature sensor", sensor.Name);
        Assert.Equal("TEMP-01", sensor.Code);
        Assert.Equal("°C", sensor.Unit);
    }
}
