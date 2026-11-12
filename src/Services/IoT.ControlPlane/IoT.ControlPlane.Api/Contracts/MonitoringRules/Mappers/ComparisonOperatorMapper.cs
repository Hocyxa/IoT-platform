using ApiOperator =
    IoT.ControlPlane.Api.Contracts.MonitoringRules.Enums.ComparisonOperatorRequest;

using DomainOperator =
    IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums.ComparisonOperator;

namespace IoT.ControlPlane.Api.Contracts.MonitoringRules.Mappers;

public static class ComparisonOperatorMapper
{
    public static DomainOperator ToDomain(ApiOperator comparisonOperator)
    {
        return comparisonOperator switch
        {
            ApiOperator.GreaterThan => DomainOperator.GreaterThan,
            ApiOperator.GreaterThanOrEqual => DomainOperator.GreaterThanOrEqual,
            ApiOperator.LessThan => DomainOperator.LessThan,
            ApiOperator.LessThanOrEqual => DomainOperator.LessThanOrEqual,
            ApiOperator.Equal => DomainOperator.Equal,
            ApiOperator.NotEqual => DomainOperator.NotEqual,
            _ => throw new ArgumentOutOfRangeException(nameof(comparisonOperator))
        };
    }

    public static ApiOperator ToApi(DomainOperator comparisonOperator)
    {
        return comparisonOperator switch
        {
            DomainOperator.GreaterThan => ApiOperator.GreaterThan,
            DomainOperator.GreaterThanOrEqual => ApiOperator.GreaterThanOrEqual,
            DomainOperator.LessThan => ApiOperator.LessThan,
            DomainOperator.LessThanOrEqual => ApiOperator.LessThanOrEqual,
            DomainOperator.Equal => ApiOperator.Equal,
            DomainOperator.NotEqual => ApiOperator.NotEqual,
            _ => throw new ArgumentOutOfRangeException(nameof(comparisonOperator))
        };
    }
}
