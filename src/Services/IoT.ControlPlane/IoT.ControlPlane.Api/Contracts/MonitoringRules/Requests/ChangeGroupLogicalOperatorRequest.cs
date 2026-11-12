using System.ComponentModel.DataAnnotations;

using IoT.ControlPlane.Api.Contracts.MonitoringRules.Enums;

namespace IoT.ControlPlane.Api.Contracts.MonitoringRules.Requests;

public sealed record ChangeGroupLogicalOperatorRequest(
    [param: EnumDataType(typeof(LogicalOperatorRequest))]
    LogicalOperatorRequest LogicalOperator
);
