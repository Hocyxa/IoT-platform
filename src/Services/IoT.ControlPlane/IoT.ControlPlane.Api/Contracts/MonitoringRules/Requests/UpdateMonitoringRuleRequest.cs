using System.ComponentModel.DataAnnotations;

using IoT.ControlPlane.Api.Contracts.MonitoringRules.Enums;

namespace IoT.ControlPlane.Api.Contracts.MonitoringRules.Requests;

public sealed record UpdateMonitoringRuleRequest(
    [param: Required]
    [param: StringLength(200, MinimumLength = 1)]
    string Name,

    [param: StringLength(1000)]
    string? Description,

    [param: EnumDataType(typeof(RuleSeverityRequest))]
    RuleSeverityRequest Severity
);
