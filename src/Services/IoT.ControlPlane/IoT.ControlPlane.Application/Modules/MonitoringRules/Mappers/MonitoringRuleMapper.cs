using IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Mappers;

public static class MonitoringRuleMapper
{
    public static MonitoringRuleDto ToDto(MonitoringRule rule)
    {
        return new MonitoringRuleDto(
            rule.Id,
            rule.DeviceId,
            rule.Name,
            rule.Description,
            rule.Severity,
            rule.IsEnabled,
            rule.MeasurementType,
            rule.CreatedAtUtc,
            rule.UpdatedAtUtc,
            rule.Sensors
                .Select(sensor => new MonitoringRuleSensorDto(sensor.SensorId))
                .ToList(),
            MapConditionGroup(rule.RootConditionGroup)
        );
    }

    private static ConditionGroupDto MapConditionGroup(ConditionGroup group)
    {
        return new ConditionGroupDto(
            group.Id,
            group.LogicalOperator,
            group.Nodes.Select(MapConditionNode).ToList()
        );
    }

    private static ConditionNodeDto MapConditionNode(ConditionNode node)
    {
        return node switch
        {
            ConditionGroup group => new ConditionNodeDto(
                group.Id,
                ConditionNodeType.Group,
                group.LogicalOperator,
                null,
                null,
                group.Nodes.Select(MapConditionNode).ToList()
            ),

            MonitoringCondition condition => new ConditionNodeDto(
                condition.Id,
                ConditionNodeType.Condition,
                null,
                condition.ComparisonOperator,
                condition.Threshold,
                []
            ),

            _ => throw new NotSupportedException(
                $"Unsupported condition node type: {node.GetType().Name}")
        };
    }
}
