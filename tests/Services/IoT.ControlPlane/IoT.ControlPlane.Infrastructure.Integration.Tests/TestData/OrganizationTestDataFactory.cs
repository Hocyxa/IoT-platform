using IoT.ControlPlane.Domain.Modules.Organizations;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.TestData;

public static class OrganizationTestDataFactory
{
    public static Organization Create(TimeProvider timeProvider)
    {
        return Organization.Create(
            Guid.NewGuid(),
            "Test organization",
            timeProvider.GetUtcNow());
    }
}
