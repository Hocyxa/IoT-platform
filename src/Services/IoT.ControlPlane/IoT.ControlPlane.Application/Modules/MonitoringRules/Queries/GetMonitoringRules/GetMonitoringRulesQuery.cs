using IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Sensors;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRules;

public sealed record GetMonitoringRulesQuery(
    Guid? DeviceId = null,
    RuleSeverity? Severity = null,
    bool? IsEnabled = null,
    MeasurementType? MeasurementType = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<MonitoringRuleDto>>;
