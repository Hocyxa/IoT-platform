
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.DeleteMonitoringRule;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Commands.DeleteMonitoringRule;

public sealed class DeleteMonitoringRuleCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _monitoringRuleRepository;
    private readonly DeleteMonitoringRuleCommandHandler _handler;

    public DeleteMonitoringRuleCommandHandlerTests()
    {
        _monitoringRuleRepository =
            Substitute.For<IMonitoringRuleRepository>();

        _handler = new DeleteMonitoringRuleCommandHandler(
            _monitoringRuleRepository);
    }

    private static MonitoringRule CreateRule()
    {
        return MonitoringRule.Create(
            Guid.NewGuid(),
            "Test rule",
            "Test description",
            RuleSeverity.Warning,
            new DateTimeOffset(
                2026, 11, 10, 10, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public async Task Handle_WhenRuleExists_DeletesRule()
    {
        // Arrange
        var rule = CreateRule();

        var command = new DeleteMonitoringRuleCommand(rule.Id);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _monitoringRuleRepository
            .GetByIdAsync(rule.Id, cancellationToken)
            .Returns(rule);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        await _monitoringRuleRepository
            .Received(1)
            .GetByIdAsync(rule.Id, cancellationToken);

        await _monitoringRuleRepository
            .Received(1)
            .DeleteAsync(rule, cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenRuleDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var ruleId = Guid.NewGuid();

        var command = new DeleteMonitoringRuleCommand(ruleId);

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
            .DeleteAsync(
                Arg.Any<MonitoringRule>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryDeleteFails_PropagatesException()
    {
        // Arrange
        var rule = CreateRule();

        var command = new DeleteMonitoringRuleCommand(rule.Id);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var expectedException = new InvalidOperationException(
            "Database delete failed.");

        _monitoringRuleRepository
            .GetByIdAsync(rule.Id, cancellationToken)
            .Returns(rule);

        _monitoringRuleRepository
            .DeleteAsync(rule, cancellationToken)
            .Returns<Task>(_ => throw expectedException);

        // Act
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        // Assert
        Assert.Same(expectedException, actualException);
    }
}
