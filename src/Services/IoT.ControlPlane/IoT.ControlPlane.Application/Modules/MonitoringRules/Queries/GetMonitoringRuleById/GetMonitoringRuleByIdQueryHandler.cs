using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Mappers;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRuleById;

public sealed class GetMonitoringRuleByIdQueryHandler(
    IMonitoringRuleRepository monitoringRuleRepository
) : IRequestHandler<GetMonitoringRuleByIdQuery, MonitoringRuleDto?>
{
    public async Task<MonitoringRuleDto?> Handle(
        GetMonitoringRuleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var rule = await monitoringRuleRepository.GetByIdAsync(
            request.MonitoringRuleId,
            cancellationToken);

        return rule is null
            ? null
            : MonitoringRuleMapper.ToDto(rule);
    }

}
