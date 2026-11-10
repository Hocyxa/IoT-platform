
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Events;
using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Domain.Tests.Modules.MonitoringRules;

public sealed class MonitoringRuleSensorTests
{
    private static MonitoringRule CreateRule(
        Guid? deviceId = null)
    {
        return MonitoringRule.Create(
            deviceId ?? Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            DateTimeOffset.UtcNow);
    }

    [Fact]
    public void AssignSensor_ShouldAssignSensorAndSetMeasurementType()
    {
        // Arrange
        var rule = CreateRule();
        var sensorId = Guid.NewGuid();
        var updatedAtUtc = rule.CreatedAtUtc.AddMinutes(1);

        // Act
        rule.AssignSensor(
            sensorId,
            rule.DeviceId,
            MeasurementType.Temperature,
            updatedAtUtc);

        // Assert
        var sensor = Assert.Single(rule.Sensors);

        Assert.Equal(sensorId, sensor.SensorId);
        Assert.Equal(MeasurementType.Temperature, rule.MeasurementType);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);

        Assert.Contains(
            rule.DomainEvents,
            x => x is MonitoringRuleUpdatedDomainEvent);
    }

    [Fact]
    public void AssignSensor_ShouldThrow_WhenSensorIdIsEmpty()
    {
        // Arrange
        var rule = CreateRule();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            rule.AssignSensor(
                Guid.Empty,
                rule.DeviceId,
                MeasurementType.Temperature,
                DateTimeOffset.UtcNow));

        // Assert
        Assert.Equal("sensorId", exception.ParamName);
        Assert.Empty(rule.Sensors);
        Assert.Null(rule.MeasurementType);
    }

    [Fact]
    public void AssignSensor_ShouldThrow_WhenSensorBelongsToAnotherDevice()
    {
        // Arrange
        var rule = CreateRule();

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.AssignSensor(
                Guid.NewGuid(),
                Guid.NewGuid(),
                MeasurementType.Temperature,
                DateTimeOffset.UtcNow));

        // Assert
        Assert.Empty(rule.Sensors);
        Assert.Null(rule.MeasurementType);
    }

    [Fact]
    public void AssignSensor_ShouldThrow_WhenSensorAlreadyAssigned()
    {
        // Arrange
        var rule = CreateRule();
        var sensorId = Guid.NewGuid();

        rule.AssignSensor(
            sensorId,
            rule.DeviceId,
            MeasurementType.Temperature,
            DateTimeOffset.UtcNow);

        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.AssignSensor(
                sensorId,
                rule.DeviceId,
                MeasurementType.Temperature,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Single(rule.Sensors);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void AssignSensor_ShouldThrow_WhenMeasurementTypeDiffers()
    {
        // Arrange
        var rule = CreateRule();

        rule.AssignSensor(
            Guid.NewGuid(),
            rule.DeviceId,
            MeasurementType.Temperature,
            DateTimeOffset.UtcNow);

        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.AssignSensor(
                Guid.NewGuid(),
                rule.DeviceId,
                MeasurementType.Humidity,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Single(rule.Sensors);
        Assert.Equal(MeasurementType.Temperature, rule.MeasurementType);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void AssignSensor_ShouldThrow_WhenMeasurementTypeIsInvalid()
    {
        // Arrange
        var rule = CreateRule();

        // Act
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            rule.AssignSensor(
                Guid.NewGuid(),
                rule.DeviceId,
                (MeasurementType)999,
                DateTimeOffset.UtcNow));

        // Assert
        Assert.Empty(rule.Sensors);
        Assert.Null(rule.MeasurementType);
    }

    [Fact]
    public void AssignSensor_ShouldAllowMultipleSensorsOfSameType()
    {
        // Arrange
        var rule = CreateRule();
        var firstSensorId = Guid.NewGuid();
        var secondSensorId = Guid.NewGuid();

        var updatedAtUtc = DateTimeOffset.UtcNow;

        // Act
        rule.AssignSensor(
            firstSensorId,
            rule.DeviceId,
            MeasurementType.Temperature,
            updatedAtUtc);

        rule.AssignSensor(
            secondSensorId,
            rule.DeviceId,
            MeasurementType.Temperature,
            updatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Equal(2, rule.Sensors.Count);
        Assert.Equal(MeasurementType.Temperature, rule.MeasurementType);
    }

    [Fact]
    public void RemoveSensor_ShouldRemoveAssignedSensor()
    {
        // Arrange
        var rule = CreateRule();
        var sensorId = Guid.NewGuid();

        rule.AssignSensor(
            sensorId,
            rule.DeviceId,
            MeasurementType.Temperature,
            DateTimeOffset.UtcNow);

        var updatedAtUtc = rule.UpdatedAtUtc.AddMinutes(1);

        // Act
        rule.RemoveSensor(sensorId, updatedAtUtc);

        // Assert
        Assert.Empty(rule.Sensors);
        Assert.Null(rule.MeasurementType);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void RemoveSensor_ShouldThrow_WhenSensorIdIsEmpty()
    {
        // Arrange
        var rule = CreateRule();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            rule.RemoveSensor(Guid.Empty, DateTimeOffset.UtcNow));

        // Assert
        Assert.Equal("sensorId", exception.ParamName);
    }

    [Fact]
    public void RemoveSensor_ShouldThrow_WhenSensorIsNotAssigned()
    {
        // Arrange
        var rule = CreateRule();
        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.RemoveSensor(Guid.NewGuid(), updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Empty(rule.Sensors);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void RemoveSensor_ShouldKeepMeasurementType_WhenOtherSensorsRemain()
    {
        // Arrange
        var rule = CreateRule();
        var firstSensorId = Guid.NewGuid();
        var secondSensorId = Guid.NewGuid();

        rule.AssignSensor(
            firstSensorId,
            rule.DeviceId,
            MeasurementType.Temperature,
            DateTimeOffset.UtcNow);

        rule.AssignSensor(
            secondSensorId,
            rule.DeviceId,
            MeasurementType.Temperature,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Act
        rule.RemoveSensor(
            firstSensorId,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        var remainingSensor = Assert.Single(rule.Sensors);

        Assert.Equal(secondSensorId, remainingSensor.SensorId);
        Assert.Equal(MeasurementType.Temperature, rule.MeasurementType);
    }

    [Fact]
    public void RemoveSensor_ShouldDisableRule_WhenLastSensorIsRemoved()
    {
        // Arrange
        var rule = CreateRule();
        var sensorId = Guid.NewGuid();

        rule.AssignSensor(
            sensorId,
            rule.DeviceId,
            MeasurementType.Temperature,
            DateTimeOffset.UtcNow);

        var groupId = rule.RootConditionGroup.Id;

        var conditionGroupId = rule.AddConditionGroup(
            groupId,
            LogicalOperator.And,
            rule.UpdatedAtUtc.AddMinutes(1));

        rule.AddCondition(
            conditionGroupId,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        rule.Enable(rule.UpdatedAtUtc.AddMinutes(1));

        // Act
        rule.RemoveSensor(
            sensorId,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.False(rule.IsEnabled);
        Assert.Empty(rule.Sensors);
        Assert.Null(rule.MeasurementType);

        Assert.Contains(
            rule.DomainEvents,
            x => x is MonitoringRuleDisabledDomainEvent);
    }

    [Fact]
    public void RemoveSensor_ShouldNotRaiseDisabledEvent_WhenRuleIsAlreadyDisabled()
    {
        // Arrange
        var rule = CreateRule();
        var sensorId = Guid.NewGuid();

        rule.AssignSensor(
            sensorId,
            rule.DeviceId,
            MeasurementType.Temperature,
            DateTimeOffset.UtcNow);

        // Act
        rule.RemoveSensor(
            sensorId,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.False(rule.IsEnabled);

        Assert.DoesNotContain(
            rule.DomainEvents,
            x => x is MonitoringRuleDisabledDomainEvent);
    }

    [Fact]
    public void RemoveSensor_ShouldRaiseUpdatedEvent()
    {
        // Arrange
        var rule = CreateRule();
        var sensorId = Guid.NewGuid();

        rule.AssignSensor(
            sensorId,
            rule.DeviceId,
            MeasurementType.Temperature,
            DateTimeOffset.UtcNow);

        // Act
        rule.RemoveSensor(
            sensorId,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Contains(
            rule.DomainEvents,
            x => x is MonitoringRuleUpdatedDomainEvent);
    }
}
