using IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Mappers;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Mappers;

public sealed class MonitoringRuleMapperTests
{
    [Fact]
    public void ToDto_ShouldMapRuleProperties()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        var rule = MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            "Monitor temperature",
            RuleSeverity.Warning,
            now);

        // Act
        var result = MonitoringRuleMapper.ToDto(rule);

        // Assert
        Assert.Equal(rule.Id, result.Id);
        Assert.Equal(rule.DeviceId, result.DeviceId);
        Assert.Equal(rule.Name, result.Name);
        Assert.Equal(rule.Description, result.Description);
        Assert.Equal(rule.Severity, result.Severity);
        Assert.Equal(rule.IsEnabled, result.IsEnabled);
        Assert.Equal(rule.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(rule.UpdatedAtUtc, result.UpdatedAtUtc);
    }

    [Fact]
    public void ToDto_ShouldMapNestedConditionTree()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        var rule = MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            now);

        var nestedGroupId = rule.AddConditionGroup(
            rule.RootConditionGroup.Id,
            LogicalOperator.Or,
            now);

        rule.AddCondition(
            nestedGroupId,
            ComparisonOperator.GreaterThan,
            50m,
            now);

        // Act
        var result = MonitoringRuleMapper.ToDto(rule);

        // Assert
        var nestedGroup = Assert.Single(result.RootConditionGroup.Nodes);

        Assert.Equal(ConditionNodeType.Group, nestedGroup.Type);
        Assert.Equal(LogicalOperator.Or, nestedGroup.LogicalOperator);

        var condition = Assert.Single(nestedGroup.Nodes);

        Assert.Equal(ConditionNodeType.Condition, condition.Type);
        Assert.Equal(ComparisonOperator.GreaterThan, condition.ComparisonOperator);
        Assert.Equal(50m, condition.Threshold);
        Assert.Empty(condition.Nodes);
    }
}
