using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

namespace IoT.ControlPlane.Domain.Modules.MonitoringRules;

public sealed class ConditionGroup : ConditionNode
{
    private readonly List<ConditionNode> _nodes = [];


    public LogicalOperator LogicalOperator { get; private set; }

    public IReadOnlyCollection<ConditionNode> Nodes
        => _nodes.AsReadOnly();

    private ConditionGroup()
    {
    }

    private ConditionGroup(
        Guid id,
        LogicalOperator logicalOperator) : base(id)
    {
        LogicalOperator = logicalOperator;
    }

    internal static ConditionGroup Create(
        LogicalOperator logicalOperator)
    {
        if (!Enum.IsDefined(logicalOperator))
        {
            throw new ArgumentOutOfRangeException(
                nameof(logicalOperator));
        }

        return new ConditionGroup(Guid.NewGuid(), logicalOperator);
    }

    internal void AddNode(ConditionNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (_nodes.Any(x => x.Id == node.Id))
        {
            throw new InvalidOperationException(
                "Condition node already exists.");
        }

        _nodes.Add(node);
    }

    internal void RemoveNode(Guid nodeId)
    {
        var node = _nodes.FirstOrDefault(x => x.Id == nodeId);

        if (node is null)
        {
            throw new InvalidOperationException(
                "Condition node not found.");
        }

        _nodes.Remove(node);
    }

    internal void ChangeLogicalOperator(
        LogicalOperator logicalOperator)
    {
        if (!Enum.IsDefined(logicalOperator))
        {
            throw new ArgumentOutOfRangeException(
                nameof(logicalOperator));
        }

        LogicalOperator = logicalOperator;
    }
}
