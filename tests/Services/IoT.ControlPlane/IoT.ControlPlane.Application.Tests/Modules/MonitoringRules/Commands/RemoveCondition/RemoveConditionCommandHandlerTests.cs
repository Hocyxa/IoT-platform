using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.RemoveCondition;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Features.RemoveCondition;

public sealed class RemoveConditionCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _repository =
        Substitute.For<IMonitoringRuleRepository>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    private readonly RemoveConditionCommandHandler _handler;

    public RemoveConditionCommandHandlerTests()
    {
        _handler = new RemoveConditionCommandHandler(
            _repository,
            _timeProvider);
    }

    [Fact]
    public async Task Handle_ShouldRemoveCondition_WhenRuleExists()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var rule = CreateRule(now);

        var condition = rule.RootConditionGroup.Nodes
            .OfType<MonitoringCondition>()
            .Single();

        _repository.GetByIdAsync(rule.Id, Arg.Any<CancellationToken>())
            .Returns(rule);

        _timeProvider.GetUtcNow().Returns(now.AddMinutes(1));

        var command = new RemoveConditionCommand(
            rule.Id,
            condition.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(rule.RootConditionGroup.Nodes);

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

        var command = new RemoveConditionCommand(
            ruleId,
            Guid.NewGuid());

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
    public async Task Handle_ShouldThrow_WhenConditionDoesNotExist()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var rule = CreateRule(now);

        _repository.GetByIdAsync(rule.Id, Arg.Any<CancellationToken>())
            .Returns(rule);

        var command = new RemoveConditionCommand(
            rule.Id,
            Guid.NewGuid());

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
    public async Task Handle_ShouldThrow_WhenRuleIsEnabled()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var rule = CreateRule(now);

        var condition = rule.RootConditionGroup.Nodes
            .OfType<MonitoringCondition>()
            .Single();

        rule.AssignSensor(
            Guid.NewGuid(),
            rule.DeviceId,
            MeasurementType.Temperature,
            now);

        rule.Enable(now);

        _repository.GetByIdAsync(rule.Id, Arg.Any<CancellationToken>())
            .Returns(rule);

        var command = new RemoveConditionCommand(
            rule.Id,
            condition.Id);

        // Act
        var act = async () =>
            await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);

        await _repository.DidNotReceive().UpdateAsync(
            Arg.Any<MonitoringRule>(),
            Arg.Any<CancellationToken>());
    }

    private static MonitoringRule CreateRule(DateTimeOffset createdAtUtc)
    {
        var rule = MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            createdAtUtc);

        rule.AddCondition(
            rule.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            50m,
            createdAtUtc);

        return rule;
    }
}
