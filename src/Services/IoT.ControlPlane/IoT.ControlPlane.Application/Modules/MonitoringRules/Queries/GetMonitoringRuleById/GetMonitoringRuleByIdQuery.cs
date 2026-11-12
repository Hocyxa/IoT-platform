using IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRuleById;

public sealed record GetMonitoringRuleByIdQuery(
    Guid MonitoringRuleId
) : IRequest<MonitoringRuleDto?>;
