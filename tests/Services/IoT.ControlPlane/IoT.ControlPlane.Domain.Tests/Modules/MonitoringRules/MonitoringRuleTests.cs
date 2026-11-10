using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Events;
using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Domain.Tests.Modules.MonitoringRules;

public sealed class MonitoringRuleTests
{
    [Fact]
    public void Create_ShouldCreateDisabledRuleAndRaiseDomainEvent()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var createdAtUtc = DateTimeOffset.UtcNow;

        // Act
        var rule = MonitoringRule.Create(
            deviceId,
            "Temperature monitoring",
            "Monitor temperature thresholds",
            RuleSeverity.Warning,
            createdAtUtc);

        // Assert
        Assert.NotEqual(Guid.Empty, rule.Id);
        Assert.Equal(deviceId, rule.DeviceId);
        Assert.Equal("Temperature monitoring", rule.Name);
        Assert.Equal("Monitor temperature thresholds", rule.Description);
        Assert.Equal(RuleSeverity.Warning, rule.Severity);

        Assert.False(rule.IsEnabled);
        Assert.Null(rule.MeasurementType);
        Assert.Empty(rule.Sensors);

        Assert.Equal(createdAtUtc, rule.CreatedAtUtc);
        Assert.Equal(createdAtUtc, rule.UpdatedAtUtc);

        var domainEvent = Assert.Single(rule.DomainEvents);

        Assert.IsType<MonitoringRuleCreatedDomainEvent>(domainEvent);

        var createdEvent =
            (MonitoringRuleCreatedDomainEvent)domainEvent;

        Assert.Equal(rule.Id, createdEvent.MonitoringRuleId);
        Assert.Equal(deviceId, createdEvent.DeviceId);
    }
    [Fact]
    public void Create_ShouldThrow_WhenDeviceIdIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            MonitoringRule.Create(
                Guid.Empty,
                "Temperature monitoring",
                null,
                RuleSeverity.Warning,
                DateTimeOffset.UtcNow));

        Assert.Equal("deviceId", exception.ParamName);
    }
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenNameIsEmpty(string name)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            MonitoringRule.Create(
                Guid.NewGuid(),
                name,
                null,
                RuleSeverity.Warning,
                DateTimeOffset.UtcNow));

        Assert.Equal("name", exception.ParamName);
    }
    [Fact]
    public void Create_ShouldThrow_WhenNameIsTooLong()
    {
        var name = new string('a', 201);

        var exception = Assert.Throws<ArgumentException>(() =>
            MonitoringRule.Create(
                Guid.NewGuid(),
                name,
                null,
                RuleSeverity.Warning,
                DateTimeOffset.UtcNow));

        Assert.Equal("name", exception.ParamName);
    }
    [Fact]
    public void Create_ShouldThrow_WhenSeverityIsInvalid()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            MonitoringRule.Create(
                Guid.NewGuid(),
                "Temperature monitoring",
                null,
                (RuleSeverity)999,
                DateTimeOffset.UtcNow));

        Assert.Equal("severity", exception.ParamName);
    }
    [Fact]
    public void AssignSensor_ShouldAssignSensorAndSetMeasurementType()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var sensorId = Guid.NewGuid();
        var createdAtUtc = DateTimeOffset.UtcNow;
        var updatedAtUtc = createdAtUtc.AddMinutes(1);

        var rule = MonitoringRule.Create(
            deviceId,
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            createdAtUtc);

        // Act
        rule.AssignSensor(
            sensorId,
            deviceId,
            MeasurementType.Temperature,
            updatedAtUtc);

        // Assert
        var sensor = Assert.Single(rule.Sensors);

        Assert.Equal(sensorId, sensor.SensorId);
        Assert.Equal(MeasurementType.Temperature, rule.MeasurementType);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);

        Assert.Equal(2, rule.DomainEvents.Count);

        Assert.IsType<MonitoringRuleUpdatedDomainEvent>(
            rule.DomainEvents.Last());
    }
}
