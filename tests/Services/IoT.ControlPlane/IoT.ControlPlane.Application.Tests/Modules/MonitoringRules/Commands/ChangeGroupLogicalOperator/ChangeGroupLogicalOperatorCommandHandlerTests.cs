using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.ChangeGroupLogicalOperator;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Features.ChangeGroupLogicalOperator;

public sealed class ChangeGroupLogicalOperatorCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _repository =
        Substitute.For<IMonitoringRuleRepository>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    private readonly ChangeGroupLogicalOperatorCommandHandler _handler;

    public ChangeGroupLogicalOperatorCommandHandlerTests()
    {
        _handler = new ChangeGroupLogicalOperatorCommandHandler(
            _repository,
            _timeProvider);
    }

    [Fact]
    public async Task Handle_ShouldChangeLogicalOperator_WhenGroupExists()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var rule = CreateRule(now);

        var nestedGroup = rule.AddConditionGroup(
            rule.RootConditionGroup.Id,
            LogicalOperator.And,
            now);

        _repository.GetByIdAsync(rule.Id, Arg.Any<CancellationToken>())
            .Returns(rule);

        _timeProvider.GetUtcNow().Returns(now.AddMinutes(1));

        var command = new ChangeGroupLogicalOperatorCommand(
            rule.Id,
            nestedGroup,
            LogicalOperator.Or);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var group = Assert.IsType<ConditionGroup>(
            Assert.Single(rule.RootConditionGroup.Nodes));

        Assert.Equal(LogicalOperator.Or, group.LogicalOperator);

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

        var command = new ChangeGroupLogicalOperatorCommand(
            ruleId,
            Guid.NewGuid(),
            LogicalOperator.Or);

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
    public async Task Handle_ShouldThrow_WhenGroupDoesNotExist()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var rule = CreateRule(now);

        _repository.GetByIdAsync(rule.Id, Arg.Any<CancellationToken>())
            .Returns(rule);

        var command = new ChangeGroupLogicalOperatorCommand(
            rule.Id,
            Guid.NewGuid(),
            LogicalOperator.Or);

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
        return MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            createdAtUtc);
    }
}
