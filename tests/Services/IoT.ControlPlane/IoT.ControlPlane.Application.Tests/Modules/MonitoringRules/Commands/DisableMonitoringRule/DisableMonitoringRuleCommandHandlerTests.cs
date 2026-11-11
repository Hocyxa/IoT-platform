using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.DisableMonitoringRule;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Features.DisableMonitoringRule;

public sealed class DisableMonitoringRuleCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _repository =
        Substitute.For<IMonitoringRuleRepository>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    private readonly DisableMonitoringRuleCommandHandler _handler;

    public DisableMonitoringRuleCommandHandlerTests()
    {
        _handler = new DisableMonitoringRuleCommandHandler(
            _repository,
            _timeProvider);
    }

    [Fact]
    public async Task Handle_ShouldDisableRule_WhenRuleIsEnabled()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var rule = CreateEnabledRule(now);

        _repository.GetByIdAsync(rule.Id, Arg.Any<CancellationToken>())
            .Returns(rule);

        _timeProvider.GetUtcNow().Returns(now.AddMinutes(1));

        var command = new DisableMonitoringRuleCommand(rule.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(rule.IsEnabled);
        Assert.Equal(now.AddMinutes(1), rule.UpdatedAtUtc);

        await _repository.Received(1).UpdateAsync(
            rule,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenRuleDoesNotExist()
    {
        // Arrange
        var ruleId = Guid.NewGuid();

        _repository.GetByIdAsync(ruleId, Arg.Any<CancellationToken>())
            .Returns((MonitoringRule?)null);

        var command = new DisableMonitoringRuleCommand(ruleId);

        // Act
        var act = async () =>
            await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);

        await _repository.DidNotReceive().UpdateAsync(
            Arg.Any<MonitoringRule>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldNotChangeUpdatedAt_WhenRuleIsAlreadyDisabled()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        var rule = MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            now);

        _repository.GetByIdAsync(rule.Id, Arg.Any<CancellationToken>())
            .Returns(rule);

        _timeProvider.GetUtcNow().Returns(now.AddMinutes(1));

        var command = new DisableMonitoringRuleCommand(rule.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(rule.IsEnabled);
        Assert.Equal(now, rule.UpdatedAtUtc);

        // UpdateAsync всё равно вызывается обработчиком.
        await _repository.Received(1).UpdateAsync(
            rule,
            Arg.Any<CancellationToken>());
    }

    private static MonitoringRule CreateEnabledRule(DateTimeOffset createdAtUtc)
    {
        var rule = MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            createdAtUtc);

        rule.AssignSensor(
            Guid.NewGuid(),
            rule.DeviceId,
            MeasurementType.Temperature,
            createdAtUtc);

        rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            50m,
            createdAtUtc);

        rule.Enable(createdAtUtc);

        return rule;
    }
}
