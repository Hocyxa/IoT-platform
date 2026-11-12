using IoT.ControlPlane.Domain.Modules.Sites;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.TestData;

public static class SiteTestDataFactory
{
    public static Site Create(
        TimeProvider timeProvider,
        Guid organizationId)
    {
        return Site.Create(
            Guid.NewGuid(),
            organizationId,
            "Test site",
            "Test address",
            timeProvider.GetUtcNow());
    }
}
