using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRuleById;

public sealed class GetMonitoringRuleByIdQueryHandler(
    IMonitoringRuleRepository monitoringRuleRepository
) : IRequestHandler<GetMonitoringRuleByIdQuery, MonitoringRuleDto?>
{
    public async Task<MonitoringRuleDto?> Handle(
        GetMonitoringRuleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var rule = await monitoringRuleRepository.GetByIdAsync(
            request.MonitoringRuleId,
            cancellationToken);

        return rule is null ? null : MapToDto(rule);
    }

    private static MonitoringRuleDto MapToDto(MonitoringRule rule)
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
