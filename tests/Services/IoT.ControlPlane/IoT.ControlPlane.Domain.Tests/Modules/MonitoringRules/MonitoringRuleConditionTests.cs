
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Events;
using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Domain.Tests.Modules.MonitoringRules;

public sealed class MonitoringRuleConditionTests
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

    private static void AssignSensor(MonitoringRule rule)
    {
        rule.AssignSensor(
            Guid.NewGuid(),
            rule.DeviceId,
            MeasurementType.Temperature,
            rule.UpdatedAtUtc.AddMinutes(1));
    }

    private static MonitoringCondition? FindCondition(
        ConditionGroup group,
        Guid conditionId)
    {
        foreach (var node in group.Nodes)
        {
            if (node is MonitoringCondition condition &&
                condition.Id == conditionId)
            {
                return condition;
            }

            if (node is ConditionGroup childGroup)
            {
                var found = FindCondition(childGroup, conditionId);

                if (found is not null)
                    return found;
            }
        }

        return null;
    }

    private static ConditionGroup? FindGroup(
        ConditionGroup parent,
        Guid groupId)
    {
        foreach (var node in parent.Nodes)
        {
            if (node is ConditionGroup group)
            {
                if (group.Id == groupId)
                    return group;

                var found = FindGroup(group, groupId);

                if (found is not null)
                    return found;
            }
        }

        return null;
    }

    private static Guid AddNestedGroup(
        MonitoringRule rule,
        Guid parentGroupId,
        LogicalOperator logicalOperator)
    {
        return rule.AddConditionGroup(
            parentGroupId,
            logicalOperator,
            rule.UpdatedAtUtc.AddMinutes(1));
    }

    // -------------------------------------------------------
    // AddCondition
    // -------------------------------------------------------

    [Fact]
    public void AddCondition_ShouldAddConditionToRootGroup()
    {
        // Arrange
        var rule = CreateRule();
        var root = rule.RootConditionGroup;
        var updatedAtUtc = rule.UpdatedAtUtc.AddMinutes(1);

        // Act
        var conditionId = rule.AddCondition(
            root.Id,
            ComparisonOperator.GreaterThan,
            30,
            updatedAtUtc);

        // Assert
        var condition = Assert.Single(root.Nodes);

        var actualCondition = Assert.IsType<MonitoringCondition>(condition);

        Assert.Equal(conditionId, actualCondition.Id);
        Assert.Equal(ComparisonOperator.GreaterThan, actualCondition.ComparisonOperator);
        Assert.Equal(30, actualCondition.Threshold);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void AddCondition_ShouldAddConditionToNestedGroup()
    {
        // Arrange
        var rule = CreateRule();

        var groupId = AddNestedGroup(
            rule,
            rule.RootConditionGroup.Id,
            LogicalOperator.Or);

        var group = FindGroup(rule.RootConditionGroup, groupId)!;

        // Act
        var conditionId = rule.AddCondition(
            groupId,
            ComparisonOperator.LessThan,
            10,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        var condition = Assert.Single(group.Nodes);

        Assert.Equal(
            conditionId,
            Assert.IsType<MonitoringCondition>(condition).Id);
    }

    [Fact]
    public void AddCondition_ShouldThrow_WhenGroupDoesNotExist()
    {
        // Arrange
        var rule = CreateRule();
        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.AddCondition(
                Guid.NewGuid(),
                ComparisonOperator.GreaterThan,
                30,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Empty(rule.RootConditionGroup.Nodes);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void AddCondition_ShouldThrow_WhenOperatorIsInvalid()
    {
        // Arrange
        var rule = CreateRule();

        // Act
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            rule.AddCondition(
                rule.RootConditionGroup.Id,
                (ComparisonOperator)999,
                30,
                rule.UpdatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Empty(rule.RootConditionGroup.Nodes);
    }

    [Fact]
    public void AddCondition_ShouldThrow_WhenRuleIsEnabled()
    {
        // Arrange
        var rule = CreateRule();

        rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        AssignSensor(rule);

        rule.Enable(rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.AddCondition(
                rule.RootConditionGroup.Id,
                ComparisonOperator.LessThan,
                10,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Single(rule.RootConditionGroup.Nodes);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    // -------------------------------------------------------
    // AddConditionGroup
    // -------------------------------------------------------

    [Fact]
    public void AddConditionGroup_ShouldAddNestedGroup()
    {
        // Arrange
        var rule = CreateRule();
        var root = rule.RootConditionGroup;
        var updatedAtUtc = rule.UpdatedAtUtc.AddMinutes(1);

        // Act
        var groupId = rule.AddConditionGroup(
            root.Id,
            LogicalOperator.Or,
            updatedAtUtc);

        // Assert
        var group = Assert.IsType<ConditionGroup>(
            Assert.Single(root.Nodes));

        Assert.Equal(groupId, group.Id);
        Assert.Equal(LogicalOperator.Or, group.LogicalOperator);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void AddConditionGroup_ShouldSupportMultipleNestingLevels()
    {
        // Arrange
        var rule = CreateRule();

        var firstGroupId = AddNestedGroup(
            rule,
            rule.RootConditionGroup.Id,
            LogicalOperator.Or);

        var secondGroupId = AddNestedGroup(
            rule,
            firstGroupId,
            LogicalOperator.And);

        var thirdGroupId = AddNestedGroup(
            rule,
            secondGroupId,
            LogicalOperator.Or);

        // Assert
        var firstGroup = FindGroup(
            rule.RootConditionGroup,
            firstGroupId);

        var secondGroup = FindGroup(
            rule.RootConditionGroup,
            secondGroupId);

        var thirdGroup = FindGroup(
            rule.RootConditionGroup,
            thirdGroupId);

        Assert.NotNull(firstGroup);
        Assert.NotNull(secondGroup);
        Assert.NotNull(thirdGroup);

        Assert.Equal(
            LogicalOperator.Or,
            firstGroup.LogicalOperator);

        Assert.Equal(
            LogicalOperator.And,
            secondGroup.LogicalOperator);

        Assert.Equal(
            LogicalOperator.Or,
            thirdGroup.LogicalOperator);
    }

    [Fact]
    public void AddConditionGroup_ShouldThrow_WhenParentGroupDoesNotExist()
    {
        // Arrange
        var rule = CreateRule();
        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.AddConditionGroup(
                Guid.NewGuid(),
                LogicalOperator.And,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Empty(rule.RootConditionGroup.Nodes);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void AddConditionGroup_ShouldThrow_WhenRuleIsEnabled()
    {
        // Arrange
        var rule = CreateRule();

        rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        AssignSensor(rule);

        rule.Enable(rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.AddConditionGroup(
                rule.RootConditionGroup.Id,
                LogicalOperator.Or,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Single(rule.RootConditionGroup.Nodes);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    // -------------------------------------------------------
    // UpdateCondition
    // -------------------------------------------------------

    [Fact]
    public void UpdateCondition_ShouldUpdateCondition()
    {
        // Arrange
        var rule = CreateRule();

        var conditionId = rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc.AddMinutes(1);

        // Act
        rule.UpdateCondition(
            conditionId,
            ComparisonOperator.LessThan,
            50,
            updatedAtUtc);

        // Assert
        var condition = FindCondition(
            rule.RootConditionGroup,
            conditionId);

        Assert.NotNull(condition);
        Assert.Equal(ComparisonOperator.LessThan, condition.ComparisonOperator);
        Assert.Equal(50, condition.Threshold);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateCondition_ShouldFindConditionInNestedGroup()
    {
        // Arrange
        var rule = CreateRule();

        var groupId = AddNestedGroup(
            rule,
            rule.RootConditionGroup.Id,
            LogicalOperator.Or);

        var conditionId = rule.AddCondition(
            groupId,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Act
        rule.UpdateCondition(
            conditionId,
            ComparisonOperator.LessThan,
            100,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        var condition = FindCondition(
            rule.RootConditionGroup,
            conditionId);

        Assert.NotNull(condition);
        Assert.Equal(ComparisonOperator.LessThan, condition.ComparisonOperator);
        Assert.Equal(100, condition.Threshold);
    }

    [Fact]
    public void UpdateCondition_ShouldThrow_WhenConditionDoesNotExist()
    {
        // Arrange
        var rule = CreateRule();
        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.UpdateCondition(
                Guid.NewGuid(),
                ComparisonOperator.GreaterThan,
                30,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateCondition_ShouldThrow_WhenOperatorIsInvalid()
    {
        // Arrange
        var rule = CreateRule();

        var conditionId = rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            rule.UpdateCondition(
                conditionId,
                (ComparisonOperator)999,
                50,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        var condition = FindCondition(
            rule.RootConditionGroup,
            conditionId);

        Assert.NotNull(condition);
        Assert.Equal(ComparisonOperator.GreaterThan, condition.ComparisonOperator);
        Assert.Equal(30, condition.Threshold);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateCondition_ShouldNotUpdate_WhenValuesAreUnchanged()
    {
        // Arrange
        var rule = CreateRule();

        var conditionId = rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc;
        var domainEventsCount = rule.DomainEvents.Count;

        // Act
        rule.UpdateCondition(
            conditionId,
            ComparisonOperator.GreaterThan,
            30,
            updatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
        Assert.Equal(domainEventsCount, rule.DomainEvents.Count);
    }

    // -------------------------------------------------------
    // RemoveCondition
    // -------------------------------------------------------

    [Fact]
    public void RemoveCondition_ShouldRemoveConditionFromRootGroup()
    {
        // Arrange
        var rule = CreateRule();

        var conditionId = rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc.AddMinutes(1);

        // Act
        rule.RemoveCondition(conditionId, updatedAtUtc);

        // Assert
        Assert.Empty(rule.RootConditionGroup.Nodes);
        Assert.Null(FindCondition(rule.RootConditionGroup, conditionId));
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void RemoveCondition_ShouldRemoveConditionFromDeeplyNestedGroup()
    {
        // Arrange
        var rule = CreateRule();

        var firstGroupId = AddNestedGroup(
            rule,
            rule.RootConditionGroup.Id,
            LogicalOperator.Or);

        var secondGroupId = AddNestedGroup(
            rule,
            firstGroupId,
            LogicalOperator.And);

        var conditionId = rule.AddCondition(
            secondGroupId,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Act
        rule.RemoveCondition(
            conditionId,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Null(FindCondition(rule.RootConditionGroup, conditionId));

        var secondGroup = FindGroup(
            rule.RootConditionGroup,
            secondGroupId);

        Assert.NotNull(secondGroup);
        Assert.Empty(secondGroup.Nodes);
    }

    [Fact]
    public void RemoveCondition_ShouldThrow_WhenConditionDoesNotExist()
    {
        // Arrange
        var rule = CreateRule();
        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.RemoveCondition(
                Guid.NewGuid(),
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void RemoveCondition_ShouldThrow_WhenRuleIsEnabled()
    {
        // Arrange
        var rule = CreateRule();

        var conditionId = rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        AssignSensor(rule);

        rule.Enable(rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.RemoveCondition(
                conditionId,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.NotNull(
            FindCondition(rule.RootConditionGroup, conditionId));

        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    // -------------------------------------------------------
    // RemoveConditionGroup
    // -------------------------------------------------------

    [Fact]
    public void RemoveConditionGroup_ShouldRemoveNestedGroup()
    {
        // Arrange
        var rule = CreateRule();

        var groupId = AddNestedGroup(
            rule,
            rule.RootConditionGroup.Id,
            LogicalOperator.Or);

        // Act
        rule.RemoveConditionGroup(
            groupId,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Null(
            FindGroup(rule.RootConditionGroup, groupId));

        Assert.Empty(rule.RootConditionGroup.Nodes);
    }

    [Fact]
    public void RemoveConditionGroup_ShouldRemoveEntireSubtree()
    {
        // Arrange
        var rule = CreateRule();

        var firstGroupId = AddNestedGroup(
            rule,
            rule.RootConditionGroup.Id,
            LogicalOperator.Or);

        var secondGroupId = AddNestedGroup(
            rule,
            firstGroupId,
            LogicalOperator.And);

        var conditionId = rule.AddCondition(
            secondGroupId,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Act
        rule.RemoveConditionGroup(
            firstGroupId,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Null(
            FindGroup(rule.RootConditionGroup, firstGroupId));

        Assert.Null(
            FindGroup(rule.RootConditionGroup, secondGroupId));

        Assert.Null(
            FindCondition(rule.RootConditionGroup, conditionId));

        Assert.Empty(rule.RootConditionGroup.Nodes);
    }

    [Fact]
    public void RemoveConditionGroup_ShouldThrow_WhenGroupDoesNotExist()
    {
        // Arrange
        var rule = CreateRule();
        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.RemoveConditionGroup(
                Guid.NewGuid(),
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void RemoveConditionGroup_ShouldNotAllowRemovingRootGroup()
    {
        // Arrange
        var rule = CreateRule();
        var rootGroupId = rule.RootConditionGroup.Id;
        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.RemoveConditionGroup(
                rootGroupId,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.Equal(rootGroupId, rule.RootConditionGroup.Id);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void RemoveConditionGroup_ShouldThrow_WhenRuleIsEnabled()
    {
        // Arrange
        var rule = CreateRule();

        // Добавляем условие в корневую группу.
        rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Создаём вложенную группу и добавляем в неё условие.
        var groupId = AddNestedGroup(
            rule,
            rule.RootConditionGroup.Id,
            LogicalOperator.Or);

        rule.AddCondition(
            groupId,
            ComparisonOperator.LessThan,
            50,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Назначаем датчик, чтобы правило можно было включить.
        AssignSensor(rule);

        rule.Enable(rule.UpdatedAtUtc.AddMinutes(1));

        var updatedAtUtc = rule.UpdatedAtUtc;

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            rule.RemoveConditionGroup(
                groupId,
                updatedAtUtc.AddMinutes(1)));

        // Assert
        Assert.NotNull(
            FindGroup(rule.RootConditionGroup, groupId));

        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);
    }

    // -------------------------------------------------------
    // Domain Events
    // -------------------------------------------------------

    [Fact]
    public void AddCondition_ShouldRaiseUpdatedDomainEvent()
    {
        // Arrange
        var rule = CreateRule();

        // Act
        rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Contains(
            rule.DomainEvents,
            x => x is MonitoringRuleUpdatedDomainEvent);
    }

    [Fact]
    public void AddConditionGroup_ShouldRaiseUpdatedDomainEvent()
    {
        // Arrange
        var rule = CreateRule();

        // Act
        rule.AddConditionGroup(
            rule.RootConditionGroup.Id,
            LogicalOperator.Or,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Contains(
            rule.DomainEvents,
            x => x is MonitoringRuleUpdatedDomainEvent);
    }

    [Fact]
    public void UpdateCondition_ShouldRaiseUpdatedDomainEvent()
    {
        // Arrange
        var rule = CreateRule();

        var conditionId = rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        rule.ClearDomainEvents();

        // Act
        rule.UpdateCondition(
            conditionId,
            ComparisonOperator.LessThan,
            50,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Single(rule.DomainEvents);
        Assert.IsType<MonitoringRuleUpdatedDomainEvent>(
            rule.DomainEvents.Single());
    }

    [Fact]
    public void RemoveCondition_ShouldRaiseUpdatedDomainEvent()
    {
        // Arrange
        var rule = CreateRule();

        var conditionId = rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            rule.UpdatedAtUtc.AddMinutes(1));

        rule.ClearDomainEvents();

        // Act
        rule.RemoveCondition(
            conditionId,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Single(rule.DomainEvents);
        Assert.IsType<MonitoringRuleUpdatedDomainEvent>(
            rule.DomainEvents.Single());
    }

    [Fact]
    public void RemoveConditionGroup_ShouldRaiseUpdatedDomainEvent()
    {
        // Arrange
        var rule = CreateRule();

        var groupId = AddNestedGroup(
            rule,
            rule.RootConditionGroup.Id,
            LogicalOperator.Or);

        rule.ClearDomainEvents();

        // Act
        rule.RemoveConditionGroup(
            groupId,
            rule.UpdatedAtUtc.AddMinutes(1));

        // Assert
        Assert.Single(rule.DomainEvents);
        Assert.IsType<MonitoringRuleUpdatedDomainEvent>(
            rule.DomainEvents.Single());
    }
}
