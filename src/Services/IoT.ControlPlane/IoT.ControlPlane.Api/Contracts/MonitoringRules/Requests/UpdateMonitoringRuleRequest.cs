using System.ComponentModel.DataAnnotations;

using IoT.ControlPlane.Api.Contracts.MonitoringRules.Enums;

namespace IoT.ControlPlane.Api.Contracts.MonitoringRules.Requests;

public sealed record UpdateMonitoringRuleRequest(
    [property: Required]
    [property: StringLength(200, MinimumLength = 1)]
    string Name,

    [property: StringLength(1000)]
    string? Description,

    [property: EnumDataType(typeof(RuleSeverityRequest))]
    RuleSeverityRequest Severity
);
