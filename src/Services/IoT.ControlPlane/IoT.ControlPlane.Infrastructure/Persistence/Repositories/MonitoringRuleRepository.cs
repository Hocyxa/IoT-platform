
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;

using Microsoft.EntityFrameworkCore;

namespace IoT.ControlPlane.Infrastructure.Persistence.Repositories;

public sealed class MonitoringRuleRepository(
    ControlPlaneDbContext dbContext)
    : IMonitoringRuleRepository
{
    public async Task AddAsync(
        MonitoringRule rule,
        CancellationToken cancellationToken)
    {
        await dbContext.MonitoringRules.AddAsync(
            rule,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        MonitoringRule rule,
        CancellationToken cancellationToken)
    {
        dbContext.MonitoringRules.Remove(rule);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MonitoringRule>> GetByDeviceIdAsync(
        Guid deviceId,
        CancellationToken cancellationToken)
    {
        var rules = await dbContext.MonitoringRules
            .AsSplitQuery()
            .Include(x => x.Sensors)
            .Include(x => x.RootConditionGroup)
            .Where(x => x.DeviceId == deviceId)
            .ToListAsync(cancellationToken);

        if (rules.Count == 0)
        {
            return rules;
        }

        await dbContext.Set<ConditionNode>()
            .FromSqlInterpolated($"""
                WITH RECURSIVE condition_tree AS (
                    SELECT node.*
                    FROM control.monitoring_rule_condition_nodes AS node
                    INNER JOIN control.monitoring_rules AS rule
                        ON rule.id = node.monitoring_rule_id
                    WHERE rule.device_id = {deviceId}

                    UNION ALL

                    SELECT child.*
                    FROM control.monitoring_rule_condition_nodes AS child
                    INNER JOIN condition_tree AS parent
                        ON child.parent_group_id = parent.id
                )
                SELECT *
                FROM condition_tree
                """)
            .ToListAsync(cancellationToken);

        return rules;
    }

    public async Task<MonitoringRule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var rule = await dbContext.MonitoringRules
            .AsSplitQuery()
            .Include(x => x.Sensors)
            .Include(x => x.RootConditionGroup)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (rule is null)
        {
            return null;
        }

        await LoadConditionTreeAsync(
            id,
            cancellationToken);

        return rule;
    }

    public async Task UpdateAsync(
        MonitoringRule rule,
        CancellationToken cancellationToken)
    {
        if (dbContext.Entry(rule).State == EntityState.Detached)
        {
            throw new InvalidOperationException(
                "Monitoring rule must be loaded before updating.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task LoadConditionTreeAsync(
        Guid ruleId,
        CancellationToken cancellationToken)
    {
        await dbContext.Set<ConditionNode>()
            .FromSqlInterpolated($"""
                WITH RECURSIVE condition_tree AS (
                    SELECT *
                    FROM control.monitoring_rule_condition_nodes
                    WHERE monitoring_rule_id = {ruleId}

                    UNION ALL

                    SELECT child.*
                    FROM control.monitoring_rule_condition_nodes AS child
                    INNER JOIN condition_tree AS parent
                        ON child.parent_group_id = parent.id
                )
                SELECT *
                FROM condition_tree
                """)
            .ToListAsync(cancellationToken);
    }
}
