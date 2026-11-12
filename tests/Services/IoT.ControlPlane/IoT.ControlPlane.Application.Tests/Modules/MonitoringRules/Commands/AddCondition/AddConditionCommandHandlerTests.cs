
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.AddCondition;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Commands.AddCondition;

public sealed class AddConditionCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _monitoringRuleRepository;
    private readonly TimeProvider _timeProvider;
    private readonly AddConditionCommandHandler _handler;

    public AddConditionCommandHandlerTests()
    {
        _monitoringRuleRepository =
            Substitute.For<IMonitoringRuleRepository>();

        _timeProvider = Substitute.For<TimeProvider>();

        _handler = new AddConditionCommandHandler(
            _monitoringRuleRepository,
            _timeProvider);
    }

    private static MonitoringRule CreateRule()
    {
        return MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            new DateTimeOffset(
                2026, 11, 10, 10, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public async Task Handle_WhenRuleExists_AddsConditionAndSavesRule()
    {
        // Arrange
        var rule = CreateRule();
        var groupId = rule.RootConditionGroup.Id;

        var command = new AddConditionCommand(
            rule.Id,
            groupId,
            ComparisonOperator.GreaterThan,
            50m);

        var updatedAtUtc = new DateTimeOffset(
            2026, 11, 10, 11, 0, 0, TimeSpan.Zero);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _monitoringRuleRepository
            .GetByIdAsync(rule.Id, cancellationToken)
            .Returns(rule);

        _timeProvider
            .GetUtcNow()
            .Returns(updatedAtUtc);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        var condition = Assert.IsType<MonitoringCondition>(
            Assert.Single(rule.RootConditionGroup.Nodes));

        Assert.Equal(ComparisonOperator.GreaterThan, condition.ComparisonOperator);
        Assert.Equal(50m, condition.Threshold);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);

        await _monitoringRuleRepository
            .Received(1)
            .UpdateAsync(rule, cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenRuleDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var ruleId = Guid.NewGuid();

        var command = new AddConditionCommand(
            ruleId,
            Guid.NewGuid(),
            ComparisonOperator.GreaterThan,
            50m);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _monitoringRuleRepository
            .GetByIdAsync(ruleId, cancellationToken)
            .Returns((MonitoringRule?)null);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        // Assert
        Assert.Contains(ruleId.ToString(), exception.Message);

        await _monitoringRuleRepository
            .DidNotReceive()
            .UpdateAsync(
                Arg.Any<MonitoringRule>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDomainRejectsCondition_PropagatesException()
    {
        // Arrange
        var rule = CreateRule();

        var command = new AddConditionCommand(
            rule.Id,
            Guid.NewGuid(),
            ComparisonOperator.GreaterThan,
            50m);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _monitoringRuleRepository
            .GetByIdAsync(rule.Id, cancellationToken)
            .Returns(rule);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        // Assert
        await _monitoringRuleRepository
            .DidNotReceive()
            .UpdateAsync(
                Arg.Any<MonitoringRule>(),
                Arg.Any<CancellationToken>());
    }
}
