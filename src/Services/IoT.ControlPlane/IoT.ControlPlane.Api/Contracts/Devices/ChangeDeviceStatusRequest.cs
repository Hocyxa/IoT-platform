namespace IoT.ControlPlane.Api.Contracts.Devices;

public sealed record ChangeDeviceStatusRequest(
    DeviceStatusRequest Status);
