
using IoT.ControlPlane.Domain.Common;

namespace IoT.ControlPlane.Domain.Modules.MonitoringRules.Events;

public sealed record MonitoringRuleEnabledDomainEvent(
    Guid MonitoringRuleId) : IDomainEvent;
