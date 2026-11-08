using ApiDeviceStatus = IoT.ControlPlane.Api.Contracts.Devices.DeviceStatusRequest;
using DomainDeviceStatus = IoT.ControlPlane.Domain.Modules.Devices.DeviceStatus;

namespace IoT.ControlPlane.Api.Contracts.Devices;

public static class DeviceStatusMapper
{
    public static DomainDeviceStatus ToDomain(ApiDeviceStatus status)
    {
        return status switch
        {
            ApiDeviceStatus.Active => DomainDeviceStatus.Active,
            ApiDeviceStatus.Maintenance => DomainDeviceStatus.Maintenance,
            ApiDeviceStatus.Disabled => DomainDeviceStatus.Disabled,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    public static ApiDeviceStatus ToApi(DomainDeviceStatus status)
    {
        return status switch
        {
            DomainDeviceStatus.Active => ApiDeviceStatus.Active,
            DomainDeviceStatus.Maintenance => ApiDeviceStatus.Maintenance,
            DomainDeviceStatus.Disabled => ApiDeviceStatus.Disabled,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }
}
