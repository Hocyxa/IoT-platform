using IoT.ControlPlane.Domain.Common;

namespace IoT.ControlPlane.Domain.Modules.MonitoringRules.Events;

public sealed record MonitoringRuleCreatedDomainEvent(
    Guid MonitoringRuleId,
    Guid DeviceId) : IDomainEvent;
