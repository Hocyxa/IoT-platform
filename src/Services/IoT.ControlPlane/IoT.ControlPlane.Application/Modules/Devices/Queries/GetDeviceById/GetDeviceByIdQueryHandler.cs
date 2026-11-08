using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.DTOs;
using IoT.ControlPlane.Application.Modules.Devices.Mappers;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Queries.GetDeviceById;

public sealed class GetDeviceByIdQueryHandler(IDeviceRepository deviceRepository) : IRequestHandler<GetDeviceByIdQuery, DeviceDto?>
{
    public async Task<DeviceDto?> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (device is null)
        {
            return null;
        }

        return DeviceMapper.ToDto(device);
    }
}
