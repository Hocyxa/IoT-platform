using IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRulesByDeviceId;

public sealed record GetMonitoringRulesByDeviceIdQuery(
    Guid DeviceId
) : IRequest<IReadOnlyList<MonitoringRuleDto>>;
