
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.UpdateMonitoringRule;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Commands.UpdateMonitoringRule;

public sealed class UpdateMonitoringRuleCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _monitoringRuleRepository;
    private readonly TimeProvider _timeProvider;

    private readonly UpdateMonitoringRuleCommandHandler _handler;

    public UpdateMonitoringRuleCommandHandlerTests()
    {
        _monitoringRuleRepository =
            Substitute.For<IMonitoringRuleRepository>();

        _timeProvider = Substitute.For<TimeProvider>();

        _handler = new UpdateMonitoringRuleCommandHandler(
            _monitoringRuleRepository,
            _timeProvider);
    }

    private static MonitoringRule CreateRule()
    {
        return MonitoringRule.Create(
            Guid.NewGuid(),
            "Original name",
            "Original description",
            RuleSeverity.Information,
            new DateTimeOffset(
                2026, 11, 10, 10, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public async Task Handle_WhenRuleExists_UpdatesAndSavesRule()
    {
        // Arrange
        var rule = CreateRule();

        var command = new UpdateMonitoringRuleCommand(
            rule.Id,
            "Updated name",
            "Updated description",
            RuleSeverity.Critical);

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
        Assert.Equal("Updated name", rule.Name);
        Assert.Equal("Updated description", rule.Description);
        Assert.Equal(RuleSeverity.Critical, rule.Severity);
        Assert.Equal(updatedAtUtc, rule.UpdatedAtUtc);

        await _monitoringRuleRepository
            .Received(1)
            .GetByIdAsync(rule.Id, cancellationToken);

        await _monitoringRuleRepository
            .Received(1)
            .UpdateAsync(rule, cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenRuleDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var ruleId = Guid.NewGuid();

        var command = new UpdateMonitoringRuleCommand(
            ruleId,
            "Updated name",
            null,
            RuleSeverity.Warning);

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
    public async Task Handle_WhenRepositoryUpdateFails_PropagatesException()
    {
        // Arrange
        var rule = CreateRule();

        var command = new UpdateMonitoringRuleCommand(
            rule.Id,
            "Updated name",
            null,
            RuleSeverity.Warning);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var expectedException = new InvalidOperationException(
            "Database update failed.");

        _monitoringRuleRepository
            .GetByIdAsync(rule.Id, cancellationToken)
            .Returns(rule);

        _timeProvider
            .GetUtcNow()
            .Returns(new DateTimeOffset(
                2026, 11, 10, 11, 0, 0, TimeSpan.Zero));

        _monitoringRuleRepository
            .UpdateAsync(rule, cancellationToken)
            .Returns<Task>(_ => throw expectedException);

        // Act
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        // Assert
        Assert.Same(expectedException, actualException);
    }
}
