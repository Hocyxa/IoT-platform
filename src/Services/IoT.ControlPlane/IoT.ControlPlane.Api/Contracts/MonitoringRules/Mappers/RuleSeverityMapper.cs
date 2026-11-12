using ApiSeverity =
    IoT.ControlPlane.Api.Contracts.MonitoringRules.Enums.RuleSeverityRequest;

using DomainSeverity =
    IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums.RuleSeverity;

namespace IoT.ControlPlane.Api.Contracts.MonitoringRules.Mappers;

public static class RuleSeverityMapper
{
    public static DomainSeverity ToDomain(ApiSeverity severity)
    {
        return severity switch
        {
            ApiSeverity.Information => DomainSeverity.Information,
            ApiSeverity.Warning => DomainSeverity.Warning,
            ApiSeverity.Critical => DomainSeverity.Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(severity))
        };
    }

    public static ApiSeverity ToApi(DomainSeverity severity)
    {
        return severity switch
        {
            DomainSeverity.Information => ApiSeverity.Information,
            DomainSeverity.Warning => ApiSeverity.Warning,
            DomainSeverity.Critical => ApiSeverity.Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(severity))
        };
    }
}
