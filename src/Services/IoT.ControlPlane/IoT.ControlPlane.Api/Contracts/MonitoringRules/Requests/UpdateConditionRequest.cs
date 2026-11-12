using System.ComponentModel.DataAnnotations;

using IoT.ControlPlane.Api.Contracts.MonitoringRules.Enums;

namespace IoT.ControlPlane.Api.Contracts.MonitoringRules.Requests;

public sealed record UpdateConditionRequest(
    [property: EnumDataType(typeof(ComparisonOperatorRequest))]
    ComparisonOperatorRequest ComparisonOperator,

    decimal Threshold
);
