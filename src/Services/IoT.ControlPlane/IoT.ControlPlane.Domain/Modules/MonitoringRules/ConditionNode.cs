namespace IoT.ControlPlane.Domain.Modules.MonitoringRules;

public abstract class ConditionNode
{
    public Guid Id { get; protected set; }
    protected ConditionNode()
    {
    }

    protected ConditionNode(Guid id)
    {
        Id = id;
    }

}
