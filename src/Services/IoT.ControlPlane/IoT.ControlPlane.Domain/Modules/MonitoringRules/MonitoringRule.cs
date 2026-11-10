using IoT.ControlPlane.Domain.Common;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Events;
using IoT.ControlPlane.Domain.Modules.Sensors;

namespace IoT.ControlPlane.Domain.Modules.MonitoringRules;

public sealed class MonitoringRule : AggregateRoot
{
    private readonly List<MonitoringRuleSensor> _sensors = [];
    private ConditionGroup _rootConditionGroup = null!;

    public Guid Id { get; private set; }

    public Guid DeviceId { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public RuleSeverity Severity { get; private set; }

    public bool IsEnabled { get; private set; }
    public MeasurementType? MeasurementType { get; private set; }
    public IReadOnlyCollection<MonitoringRuleSensor> Sensors
        => _sensors.AsReadOnly();
    public ConditionGroup RootConditionGroup => _rootConditionGroup;
    private MonitoringRule()
    {

    }
    private MonitoringRule(
       Guid id,
       Guid deviceId,
       string name,
       string? description,
       RuleSeverity severity,
       DateTimeOffset createdAtUtc)
       : base(createdAtUtc)
    {
        Id = id;
        DeviceId = deviceId;
        Name = name;
        Description = description;
        Severity = severity;
        IsEnabled = false;
        _rootConditionGroup = ConditionGroup.Create(LogicalOperator.And);
    }
    private void EnsureDisabled()
    {
        if (IsEnabled)
        {
            throw new InvalidOperationException(
                "Cannot modify the condition tree while the rule is enabled.");
        }
    }
    public static MonitoringRule Create(
        Guid deviceId,
        string name,
        string? description,
        RuleSeverity severity,
        DateTimeOffset createdAtUtc)
    {
        if (deviceId == Guid.Empty)
        {
            throw new ArgumentException(
                "Device ID cannot be empty.",
                nameof(deviceId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Rule name cannot be empty.",
                nameof(name));
        }

        name = name.Trim();

        if (name.Length > 200)
        {
            throw new ArgumentException(
                "Rule name cannot exceed 200 characters.",
                nameof(name));
        }

        description = description?.Trim();

        if (description?.Length > 1000)
        {
            throw new ArgumentException(
                "Rule description cannot exceed 1000 characters.",
                nameof(description));
        }

        if (!Enum.IsDefined(severity))
        {
            throw new ArgumentOutOfRangeException(
                nameof(severity));
        }

        var rule = new MonitoringRule(
            Guid.NewGuid(),
            deviceId,
            name,
            description,
            severity,
            createdAtUtc);

        rule.AddDomainEvent(
            new MonitoringRuleCreatedDomainEvent(
                rule.Id,
                rule.DeviceId));

        return rule;
    }

    public void AssignSensor(
        Guid sensorId,
        Guid sensorDeviceId,
        MeasurementType sensorType,
        DateTimeOffset updatedAtUtc)
    {
        if (sensorId == Guid.Empty)
        {
            throw new ArgumentException(
                "Sensor ID cannot be empty.",
                nameof(sensorId));
        }

        if (sensorDeviceId != DeviceId)
        {
            throw new InvalidOperationException(
                "Sensor must belong to the same device as the monitoring rule.");
        }

        if (!Enum.IsDefined(sensorType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(sensorType));
        }

        if (_sensors.Any(x => x.SensorId == sensorId))
        {
            throw new InvalidOperationException(
                "Sensor is already assigned to this monitoring rule.");
        }

        if (MeasurementType.HasValue &&
            MeasurementType.Value != sensorType)
        {
            throw new InvalidOperationException(
                "All sensors in a monitoring rule must have the same measurement type.");
        }

        _sensors.Add(new MonitoringRuleSensor(sensorId));

        MeasurementType ??= sensorType;

        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(
            new MonitoringRuleUpdatedDomainEvent(Id));
    }

    public void UpdateDetails(
        string name,
        string? description,
        RuleSeverity severity,
        DateTimeOffset updatedAtUtc)
    {
        if (!Enum.IsDefined(severity))
        {
            throw new ArgumentOutOfRangeException(nameof(severity));
        }
        if (Name == name &&
            Description == description &&
            Severity == severity)
        {
            return;
        }
        name = name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Monitoring rule name cannot be empty.",
                nameof(name));
        }

        if (name.Length > 200)
        {
            throw new ArgumentException(
                "Monitoring rule name cannot exceed 200 characters.",
                nameof(name));
        }

        description = description?.Trim();

        if (description?.Length > 1000)
        {
            throw new ArgumentException(
                "Monitoring rule description cannot exceed 1000 characters.",
                nameof(description));
        }

        if (Name == name &&
            Description == description &&
            Severity == severity)
        {
            return;
        }

        Name = name;
        Description = description;
        Severity = severity;

        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(new MonitoringRuleUpdatedDomainEvent(Id));
    }

    public void RemoveSensor(
        Guid sensorId,
        DateTimeOffset updatedAtUtc)
    {
        if (sensorId == Guid.Empty)
        {
            throw new ArgumentException(
                "Sensor ID cannot be empty.",
                nameof(sensorId));
        }

        var sensor = _sensors.FirstOrDefault(
            x => x.SensorId == sensorId);

        if (sensor is null)
        {
            throw new InvalidOperationException(
                "Sensor is not assigned to this monitoring rule.");
        }

        _sensors.Remove(sensor);

        if (_sensors.Count == 0)
        {
            MeasurementType = null;

            if (IsEnabled)
            {
                IsEnabled = false;

                AddDomainEvent(
                    new MonitoringRuleDisabledDomainEvent(Id));
            }
        }
        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(
            new MonitoringRuleUpdatedDomainEvent(Id));
    }


    private ConditionGroup? FindGroup(Guid groupId)
    {
        if (_rootConditionGroup.Id == groupId)
        {
            return _rootConditionGroup;
        }

        foreach (var node in _rootConditionGroup.Nodes)
        {
            if (node is not ConditionGroup group)
            {
                continue;
            }

            var found = FindGroup(group, groupId);

            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private static ConditionGroup? FindGroup(
        ConditionGroup parent,
        Guid groupId)
    {
        if (parent.Id == groupId)
        {
            return parent;
        }

        foreach (var node in parent.Nodes)
        {
            if (node is ConditionGroup child)
            {
                var found = FindGroup(child, groupId);

                if (found is not null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    public Guid AddCondition(
        Guid groupId,
        ComparisonOperator comparisonOperator,
        decimal threshold,
        DateTimeOffset updatedAtUtc)
    {
        EnsureDisabled();

        var group = FindGroup(groupId);

        if (group is null)
        {
            throw new InvalidOperationException(
                "Condition group not found.");
        }

        var condition = MonitoringCondition.Create(
            comparisonOperator,
            threshold);

        group.AddNode(condition);

        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(
            new MonitoringRuleUpdatedDomainEvent(Id));

        return condition.Id;
    }
    public Guid AddConditionGroup(
        Guid parentGroupId,
        LogicalOperator logicalOperator,
        DateTimeOffset updatedAtUtc)
    {
        EnsureDisabled();
        var parentGroup = FindGroup(parentGroupId);

        if (parentGroup is null)
        {
            throw new InvalidOperationException(
                "Parent condition group not found.");
        }

        var group = ConditionGroup.Create(logicalOperator);

        parentGroup.AddNode(group);

        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(
            new MonitoringRuleUpdatedDomainEvent(Id));

        return group.Id;
    }

    private MonitoringCondition? FindCondition(Guid conditionId)
    {
        return FindCondition(_rootConditionGroup, conditionId);
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
                {
                    return found;
                }
            }
        }

        return null;
    }

    public void UpdateCondition(
        Guid conditionId,
        ComparisonOperator comparisonOperator,
        decimal threshold,
        DateTimeOffset updatedAtUtc)
    {
        if (!Enum.IsDefined(comparisonOperator))
        {
            throw new ArgumentOutOfRangeException(nameof(comparisonOperator));
        }

        var condition = FindCondition(conditionId);

        if (condition is null)
        {
            throw new InvalidOperationException(
                "Monitoring condition not found.");
        }

        if (condition.ComparisonOperator == comparisonOperator &&
            condition.Threshold == threshold)
        {
            return;
        }
        condition.Update(comparisonOperator, threshold);

        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(
            new MonitoringRuleUpdatedDomainEvent(Id));
    }
    private ConditionGroup? FindParentGroup(Guid nodeId)
    {
        return FindParentGroup(_rootConditionGroup, nodeId);
    }

    private static ConditionGroup? FindParentGroup(
        ConditionGroup parent,
        Guid nodeId)
    {
        foreach (var node in parent.Nodes)
        {
            if (node.Id == nodeId)
            {
                return parent;
            }

            if (node is ConditionGroup childGroup)
            {
                var found = FindParentGroup(childGroup, nodeId);

                if (found is not null)
                {
                    return found;
                }
            }
        }

        return null;
    }
    public void RemoveCondition(
        Guid conditionId,
        DateTimeOffset updatedAtUtc)
    {
        EnsureDisabled();
        var parentGroup = FindParentGroup(conditionId);

        if (parentGroup is null ||
            !parentGroup.Nodes.Any(
                x => x is MonitoringCondition && x.Id == conditionId))
        {
            throw new InvalidOperationException(
                "Monitoring condition not found.");
        }

        parentGroup.RemoveNode(conditionId);

        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(
            new MonitoringRuleUpdatedDomainEvent(Id));
    }

    public void RemoveConditionGroup(
        Guid groupId,
        DateTimeOffset updatedAtUtc)
    {
        EnsureDisabled();
        var parentGroup = FindParentGroup(groupId);

        if (parentGroup is null ||
            !parentGroup.Nodes.Any(
                x => x is ConditionGroup && x.Id == groupId))
        {
            throw new InvalidOperationException(
                "Condition group not found.");
        }

        // Удаляем группу целиком вместе со всеми вложенными узлами.
        parentGroup.RemoveNode(groupId);

        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(
            new MonitoringRuleUpdatedDomainEvent(Id));
    }

    public void ChangeGroupLogicalOperator(
        Guid groupId,
        LogicalOperator logicalOperator,
        DateTimeOffset updatedAtUtc)
    {
        var group = FindGroup(groupId);
        if (group is null)
        {
            throw new InvalidOperationException(
                "Condition group not found.");
        }

        if (!Enum.IsDefined(logicalOperator))
        {
            throw new ArgumentOutOfRangeException(nameof(logicalOperator));
        }

        if (group.LogicalOperator == logicalOperator)
        {
            return;
        }

        group.ChangeLogicalOperator(logicalOperator);

        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(
            new MonitoringRuleUpdatedDomainEvent(Id));
    }

    private static bool HasValidConditionTree(ConditionGroup group)
    {
        if (group.Nodes.Count == 0)
        {
            return false;
        }

        foreach (var node in group.Nodes)
        {
            if (node is ConditionGroup childGroup &&
                !HasValidConditionTree(childGroup))
            {
                return false;
            }
        }

        return true;
    }

    public void Enable(DateTimeOffset updatedAtUtc)
    {
        if (IsEnabled)
        {
            return;
        }

        if (_sensors.Count == 0)
        {
            throw new InvalidOperationException(
                "Cannot enable a rule without assigned sensors.");
        }

        if (!HasValidConditionTree(_rootConditionGroup))
        {
            throw new InvalidOperationException(
                "Cannot enable a rule with an invalid condition tree.");
        }

        IsEnabled = true;

        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(
            new MonitoringRuleEnabledDomainEvent(Id));
    }
    public void Disable(DateTimeOffset updatedAtUtc)
    {
        if (!IsEnabled)
        {
            return;
        }

        IsEnabled = false;

        MarkAsUpdated(updatedAtUtc);

        AddDomainEvent(
            new MonitoringRuleDisabledDomainEvent(Id));
    }

}
