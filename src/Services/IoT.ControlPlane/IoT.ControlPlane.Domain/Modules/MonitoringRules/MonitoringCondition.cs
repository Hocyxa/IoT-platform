using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

namespace IoT.ControlPlane.Domain.Modules.MonitoringRules;

public sealed class MonitoringCondition : ConditionNode
{

    public ComparisonOperator ComparisonOperator { get; private set; }

    public decimal Threshold { get; private set; }

    private MonitoringCondition()
    {
    }

    private MonitoringCondition(
        Guid id,
        ComparisonOperator comparisonOperator,
        decimal threshold) : base(id)
    {
        ComparisonOperator = comparisonOperator;
        Threshold = threshold;
    }

    internal static MonitoringCondition Create(
        ComparisonOperator comparisonOperator,
        decimal threshold)
    {
        if (!Enum.IsDefined(comparisonOperator))
        {
            throw new ArgumentOutOfRangeException(
                nameof(comparisonOperator));
        }

        return new MonitoringCondition(
            Guid.NewGuid(),
            comparisonOperator,
            threshold);
    }

    internal void Update(
        ComparisonOperator comparisonOperator,
        decimal threshold)
    {
        if (!Enum.IsDefined(comparisonOperator))
        {
            throw new ArgumentOutOfRangeException(
                nameof(comparisonOperator));
        }

        ComparisonOperator = comparisonOperator;
        Threshold = threshold;
    }
}
