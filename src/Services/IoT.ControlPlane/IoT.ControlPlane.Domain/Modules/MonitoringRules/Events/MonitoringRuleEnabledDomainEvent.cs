
using IoT.ControlPlane.Domain.Common;

namespace IoT.ControlPlane.Domain.Modules.MonitoringRules.Events;

public sealed record MonitoringRuleDisabledDomainEvent(
    Guid MonitoringRuleId) : IDomainEvent;
