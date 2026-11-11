using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.EnableMonitoringRule;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Features.EnableMonitoringRule;

public sealed class EnableMonitoringRuleCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _repository =
        Substitute.For<IMonitoringRuleRepository>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    private readonly EnableMonitoringRuleCommandHandler _handler;

    public EnableMonitoringRuleCommandHandlerTests()
    {
        _handler = new EnableMonitoringRuleCommandHandler(
            _repository,
            _timeProvider);
    }

    [Fact]
    public async Task Handle_ShouldEnableRule_WhenRuleIsValid()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var rule = CreateValidRule(now);

        _repository.GetByIdAsync(rule.Id, Arg.Any<CancellationToken>())
            .Returns(rule);

        _timeProvider.GetUtcNow().Returns(now.AddMinutes(1));

        var command = new EnableMonitoringRuleCommand(rule.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(rule.IsEnabled);
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

        var command = new EnableMonitoringRuleCommand(ruleId);

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
    public async Task Handle_ShouldThrow_WhenRuleHasNoSensors()
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

        var command = new EnableMonitoringRuleCommand(rule.Id);

        // Act
        var act = async () =>
            await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);

        Assert.False(rule.IsEnabled);

        await _repository.DidNotReceive().UpdateAsync(
            Arg.Any<MonitoringRule>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenConditionTreeIsInvalid()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        var rule = MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            now);

        rule.AssignSensor(
            Guid.NewGuid(),
            rule.DeviceId,
            MeasurementType.Temperature,
            now);

        // Корневая группа пуста, поэтому дерево условий невалидно.
        _repository.GetByIdAsync(rule.Id, Arg.Any<CancellationToken>())
            .Returns(rule);

        var command = new EnableMonitoringRuleCommand(rule.Id);

        // Act
        var act = async () =>
            await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);

        Assert.False(rule.IsEnabled);

        await _repository.DidNotReceive().UpdateAsync(
            Arg.Any<MonitoringRule>(),
            Arg.Any<CancellationToken>());
    }

    private static MonitoringRule CreateValidRule(DateTimeOffset createdAtUtc)
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

        return rule;
    }
}
