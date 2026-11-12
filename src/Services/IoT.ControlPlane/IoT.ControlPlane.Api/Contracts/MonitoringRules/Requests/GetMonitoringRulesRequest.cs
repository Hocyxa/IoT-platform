using System.ComponentModel.DataAnnotations;

using IoT.ControlPlane.Api.Contracts.MonitoringRules.Enums;
using IoT.ControlPlane.Api.Contracts.Sensors;

namespace IoT.ControlPlane.Api.Contracts.MonitoringRules.Requests;

public sealed record GetMonitoringRulesRequest(
    Guid? DeviceId = null,

    [EnumDataType(typeof(RuleSeverityRequest))]
    RuleSeverityRequest? Severity = null,

    bool? IsEnabled = null,

    [EnumDataType(typeof(MeasurementTypeRequest))]
    MeasurementTypeRequest? MeasurementType = null,

    [Range(1, int.MaxValue)]
    int Page = 1,

    [Range(1, 100)]
    int PageSize = 20
);
