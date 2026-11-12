using IoT.ControlPlane.Domain.Modules.Devices;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.TestData;

public static class DeviceTestDataFactory
{
    public static Device Create(
        TimeProvider timeProvider,
        Guid siteId,
        int ind)
    {
        return Device.Create(
            Guid.NewGuid(),
            siteId,
            $"Test device {ind}",
            $"DEVICE-{ind}",
            timeProvider.GetUtcNow());
    }
}
