using ApiOperator =
    IoT.ControlPlane.Api.Contracts.MonitoringRules.Enums.LogicalOperatorRequest;

using DomainOperator =
    IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums.LogicalOperator;

namespace IoT.ControlPlane.Api.Contracts.MonitoringRules.Mappers;

public static class LogicalOperatorMapper
{
    public static DomainOperator ToDomain(ApiOperator logicalOperator)
    {
        return logicalOperator switch
        {
            ApiOperator.And => DomainOperator.And,
            ApiOperator.Or => DomainOperator.Or,
            _ => throw new ArgumentOutOfRangeException(nameof(logicalOperator))
        };
    }

    public static ApiOperator ToApi(DomainOperator logicalOperator)
    {
        return logicalOperator switch
        {
            DomainOperator.And => ApiOperator.And,
            DomainOperator.Or => ApiOperator.Or,
            _ => throw new ArgumentOutOfRangeException(nameof(logicalOperator))
        };
    }
}
