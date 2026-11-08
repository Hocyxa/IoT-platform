using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.DTOs;
using IoT.ControlPlane.Application.Modules.Devices.Mappers;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Queries.GetDevicesBySite;


public sealed class GetDevicesBySiteQueryHandler(IDeviceRepository deviceRepository) : IRequestHandler<GetDevicesBySiteQuery, IReadOnlyList<DeviceDto>>
{
    public async Task<IReadOnlyList<DeviceDto>> Handle(GetDevicesBySiteQuery request, CancellationToken cancellationToken)
    {
        var devices = await deviceRepository.GetBySiteIdAsync(request.SiteId, cancellationToken);
        return devices.Select(DeviceMapper.ToDto).ToList();
    }
}
