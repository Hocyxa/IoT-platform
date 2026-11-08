using IoT.ControlPlane.Application.Modules.Devices.DTOs;
using IoT.ControlPlane.Domain.Modules.Devices;

namespace IoT.ControlPlane.Application.Modules.Devices.Mappers;

public static class DeviceMapper
{
    public static DeviceDto ToDto(Device device)
    {
        return new DeviceDto(
            device.Id,
            device.SiteId,
            device.Name,
            device.SerialNumber,
            device.Status,
            device.CreatedAtUtc,
            device.UpdatedAtUtc);
    }
}
