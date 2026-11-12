
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.UpdateCondition;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Commands.UpdateCondition;

public sealed class UpdateConditionCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _monitoringRuleRepository;
    private readonly TimeProvider _timeProvider;
    private readonly UpdateConditionCommandHandler _handler;

    public UpdateConditionCommandHandlerTests()
    {
        _monitoringRuleRepository =
            Substitute.For<IMonitoringRuleRepository>();

        _timeProvider = Substitute.For<TimeProvider>();

        _handler = new UpdateConditionCommandHandler(
            _monitoringRuleRepository,
            _timeProvider);
    }

    private static MonitoringRule CreateRuleWithCondition(
        out Guid conditionId)
    {
        var createdAtUtc = new DateTimeOffset(
            2026, 11, 10, 10, 0, 0, TimeSpan.Zero);

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

        var condition = Assert.IsType<MonitoringCondition>(
            Assert.Single(rule.RootConditionGroup.Nodes));

        conditionId = condition.Id;

        return rule;
    }

    [Fact]
    public async Task Handle_WhenRuleAndConditionExist_UpdatesConditionAndSavesRule()
    {
        // Arrange
        var rule = CreateRuleWithCondition(out var conditionId);

        var command = new UpdateConditionCommand(
            rule.Id,
            conditionId,
            ComparisonOperator.LessThan,
            80m);

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

        Assert.Equal(ComparisonOperator.LessThan, condition.ComparisonOperator);
        Assert.Equal(80m, condition.Threshold);
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

        var command = new UpdateConditionCommand(
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
    public async Task Handle_WhenConditionDoesNotExist_PropagatesDomainException()
    {
        // Arrange
        var rule = MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            new DateTimeOffset(
                2026, 11, 10, 10, 0, 0, TimeSpan.Zero));

        var command = new UpdateConditionCommand(
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
