using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.TestData;

public static class MonitoringRuleTestDataFactory
{
    public static MonitoringRule Create(
    TimeProvider timeProvider,
    Guid deviceId,
    RuleSeverity severity,
    int index)
    {
        return MonitoringRule.Create(
            deviceId,
            $"Test monitoring rule {index}",
            $"Description for rule {index}",
            severity,
            timeProvider.GetUtcNow());
    }
}
