using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.DTOs;
using IoT.ControlPlane.Application.Modules.Devices.Mappers;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Queries.GetDevices;


public sealed class GetDevicesQueryHandler(IDeviceRepository deviceRepository) : IRequestHandler<GetDevicesQuery, IReadOnlyList<DeviceDto>>
{
    public async Task<IReadOnlyList<DeviceDto>> Handle(GetDevicesQuery request, CancellationToken cancellationToken)
    {
        var devices = await deviceRepository.GetAllAsync(cancellationToken);
        return devices.Select(DeviceMapper.ToDto).ToList();
    }
}
