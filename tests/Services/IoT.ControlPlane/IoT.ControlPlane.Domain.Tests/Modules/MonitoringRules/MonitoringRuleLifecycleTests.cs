
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Events;
using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Domain.Tests.Modules.MonitoringRules;

public sealed class MonitoringRuleLifecycleTests
{
    private static MonitoringRule CreateRule()
    {
        return MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            DateTimeOffset.UtcNow);
    }

    private static MonitoringRule CreateReadyRule()
    {
        var rule = CreateRule();

        rule.AssignSensor(
            Guid.NewGuid(),
            rule.DeviceId,
            MeasurementType.Temperature,
            rule.CreatedAtUtc.AddMinutes(1));

        rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        return rule;
    }

    [Fact]
    public void Enable_ShouldEnableRuleAndRaiseDomainEvent()
    {
        // Arrange
        var rule = CreateReadyRule();
        var updatedAtUtc = rule.UpdatedAtUtc.AddMinutes(1);

        // Act
        rule.Enable(updatedAtUtc);

        // Assert
        Assert.True(rule.IsEnabled);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);

        Assert.Contains(
            rule.DomainEvents,
            x => x is MonitoringRuleEnabledDomainEvent);
    }

    [Fact]
    public void Enable_ShouldThrow_WhenNoSensorsAssigned()
    {
        // Arrange
        var rule = CreateRule();

        rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(
            () => rule.Enable(updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.False(rule.IsEnabled);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);

        Assert.DoesNotContain(
            rule.DomainEvents,
            x => x is MonitoringRuleEnabledDomainEvent);
    }

    [Fact]
    public void Enable_ShouldThrow_WhenConditionTreeIsEmpty()
    {
        // Arrange
        var rule = CreateRule();

        rule.AssignSensor(
            Guid.NewGuid(),
            rule.DeviceId,
            MeasurementType.Temperature,
            rule.CreatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(
            () => rule.Enable(updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.False(rule.IsEnabled);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void Enable_ShouldThrow_WhenNestedGroupIsEmpty()
    {
        // Arrange
        var rule = CreateRule();

        rule.AssignSensor(
            Guid.NewGuid(),
            rule.DeviceId,
            MeasurementType.Temperature,
            rule.CreatedAtUtc.AddMinutes(1));

        rule.AddConditionGroup(
            rule.RootConditionGroup.Id,
            LogicalOperator.And,
            rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(
            () => rule.Enable(updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.False(rule.IsEnabled);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void Enable_ShouldNotChangeAnything_WhenAlreadyEnabled()
    {
        // Arrange
        var rule = CreateReadyRule();

        rule.Enable(rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc;
        var domainEventsCount = rule.DomainEvents.Count;

        // Act
        rule.Enable(updatedAtUtc.AddMinutes(1));

        // Assert
        Assert.True(rule.IsEnabled);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
        Assert.Equal(domainEventsCount, rule.DomainEvents.Count);
    }

    [Fact]
    public void Disable_ShouldDisableRuleAndRaiseDomainEvent()
    {
        // Arrange
        var rule = CreateReadyRule();

        rule.Enable(rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc.AddMinutes(1);

        // Act
        rule.Disable(updatedAtUtc);

        // Assert
        Assert.False(rule.IsEnabled);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);

        Assert.Contains(
            rule.DomainEvents,
            x => x is MonitoringRuleDisabledDomainEvent);
    }

    [Fact]
    public void Disable_ShouldNotChangeAnything_WhenAlreadyDisabled()
    {
        // Arrange
        var rule = CreateRule();

        var updatedAtUtc = rule.UpdatedAtUtc;
        var domainEventsCount = rule.DomainEvents.Count;

        // Act
        rule.Disable(updatedAtUtc.AddMinutes(1));

        // Assert
        Assert.False(rule.IsEnabled);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
        Assert.Equal(domainEventsCount, rule.DomainEvents.Count);
    }

    [Fact]
    public void ChangeGroupLogicalOperator_ShouldChangeOperator()
    {
        // Arrange
        var rule = CreateRule();
        var groupId = rule.RootConditionGroup.Id;

        var updatedAtUtc = rule.UpdatedAtUtc.AddMinutes(1);

        // Act
        rule.ChangeGroupLogicalOperator(
            groupId,
            LogicalOperator.Or,
            updatedAtUtc);

        // Assert
        Assert.Equal(
            LogicalOperator.Or,
            rule.RootConditionGroup.LogicalOperator);

        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);

        Assert.Contains(
            rule.DomainEvents,
            x => x is MonitoringRuleUpdatedDomainEvent);
    }

    [Fact]
    public void ChangeGroupLogicalOperator_ShouldThrow_WhenGroupDoesNotExist()
    {
        // Arrange
        var rule = CreateRule();
        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.ChangeGroupLogicalOperator(
                Guid.NewGuid(),
                LogicalOperator.Or,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void ChangeGroupLogicalOperator_ShouldThrow_WhenOperatorIsInvalid()
    {
        // Arrange
        var rule = CreateRule();
        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            rule.ChangeGroupLogicalOperator(
                rule.RootConditionGroup.Id,
                (LogicalOperator)999,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Equal(
            LogicalOperator.And,
            rule.RootConditionGroup.LogicalOperator);

        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void ChangeGroupLogicalOperator_ShouldNotUpdate_WhenOperatorIsSame()
    {
        // Arrange
        var rule = CreateRule();

        var updatedAtUtc = rule.UpdatedAtUtc;
        var domainEventsCount = rule.DomainEvents.Count;

        // Act
        rule.ChangeGroupLogicalOperator(
            rule.RootConditionGroup.Id,
            LogicalOperator.And,
            updatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Equal(
            LogicalOperator.And,
            rule.RootConditionGroup.LogicalOperator);

        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
        Assert.Equal(domainEventsCount, rule.DomainEvents.Count);
    }
}
