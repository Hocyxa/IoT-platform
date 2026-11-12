using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Mappers;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRules;

public sealed class GetMonitoringRulesQueryHandler(
    IMonitoringRuleRepository monitoringRuleRepository
) : IRequestHandler<GetMonitoringRulesQuery, PagedResult<MonitoringRuleDto>>
{
    public async Task<PagedResult<MonitoringRuleDto>> Handle(
        GetMonitoringRulesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page < 1)
            throw new ArgumentOutOfRangeException(nameof(request.Page));

        if (request.PageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(request.PageSize));

        var (items, totalCount) = await monitoringRuleRepository.GetFilteredAsync(
            request.DeviceId,
            request.Severity,
            request.IsEnabled,
            request.MeasurementType,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<MonitoringRuleDto>(
            items.Select(MonitoringRuleMapper.ToDto).ToList(),
            totalCount,
            request.Page,
            request.PageSize);
    }


}
