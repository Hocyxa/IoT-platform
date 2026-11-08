using IoT.ControlPlane.Application.Modules.Devices.DTOs;

namespace IoT.ControlPlane.Api.Contracts.Devices;

public static class DeviceResponseMapper
{
    public static DeviceResponse ToResponse(DeviceDto device)
    {
        return new DeviceResponse(
            device.Id,
            device.SiteId,
            device.Name,
            device.SerialNumber,
            DeviceStatusMapper.ToApi(device.Status).ToString(),
            device.CreatedAtUtc,
            device.UpdatedAtUtc);
    }
}
