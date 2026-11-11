using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Mappers;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRulesByDeviceId;

public sealed class GetMonitoringRulesByDeviceIdQueryHandler(
    IMonitoringRuleRepository monitoringRuleRepository
) : IRequestHandler<GetMonitoringRulesByDeviceIdQuery, IReadOnlyList<MonitoringRuleDto>>
{
    public async Task<IReadOnlyList<MonitoringRuleDto>> Handle(
        GetMonitoringRulesByDeviceIdQuery request,
        CancellationToken cancellationToken)
    {
        var rules = await monitoringRuleRepository.GetByDeviceIdAsync(
            request.DeviceId,
            cancellationToken);

        return rules
            .Select(MonitoringRuleMapper.ToDto)
            .ToList();
    }

}
