using System.ComponentModel.DataAnnotations;

using IoT.ControlPlane.Api.Contracts.MonitoringRules.Enums;

namespace IoT.ControlPlane.Api.Contracts.MonitoringRules.Requests;

public sealed record ChangeGroupLogicalOperatorRequest(
    [property: EnumDataType(typeof(LogicalOperatorRequest))]
    LogicalOperatorRequest LogicalOperator
);
