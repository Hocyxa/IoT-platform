using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.DTOs;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRuleById;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Features.GetMonitoringRuleById;

public sealed class GetMonitoringRuleByIdQueryHandlerTests
{
    private readonly IMonitoringRuleRepository _repository =
        Substitute.For<IMonitoringRuleRepository>();

    private readonly GetMonitoringRuleByIdQueryHandler _handler;

    public GetMonitoringRuleByIdQueryHandlerTests()
    {
        _handler = new GetMonitoringRuleByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnRule_WhenRuleExists()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var rule = CreateRuleWithConditions(now);

        _repository.GetByIdAsync(rule.Id, Arg.Any<CancellationToken>())
            .Returns(rule);

        var query = new GetMonitoringRuleByIdQuery(rule.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(rule.Id, result.Id);
        Assert.Equal(rule.DeviceId, result.DeviceId);
        Assert.Equal(rule.Name, result.Name);
        Assert.Equal(rule.Severity, result.Severity);
        Assert.False(result.IsEnabled);

        Assert.Single(result.RootConditionGroup.Nodes);

        var nestedGroup = Assert.Single(result.RootConditionGroup.Nodes);
        Assert.Equal(ConditionNodeType.Group, nestedGroup.Type);
        Assert.Equal(LogicalOperator.Or, nestedGroup.LogicalOperator);

        var condition = Assert.Single(nestedGroup.Nodes);
        Assert.Equal(ConditionNodeType.Condition, condition.Type);
        Assert.Equal(ComparisonOperator.GreaterThan, condition.ComparisonOperator);
        Assert.Equal(50m, condition.Threshold);

        await _repository.Received(1).GetByIdAsync(
            rule.Id,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRuleDoesNotExist()
    {
        // Arrange
        var ruleId = Guid.NewGuid();

        _repository.GetByIdAsync(ruleId, Arg.Any<CancellationToken>())
            .Returns((MonitoringRule?)null);

        var query = new GetMonitoringRuleByIdQuery(ruleId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);

        await _repository.Received(1).GetByIdAsync(
            ruleId,
            Arg.Any<CancellationToken>());
    }

    private static MonitoringRule CreateRuleWithConditions(
        DateTimeOffset createdAtUtc)
    {
        var rule = MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            "Monitor temperature thresholds",
            RuleSeverity.Warning,
            createdAtUtc);

        var nestedGroupId = rule.AddConditionGroup(
            rule.RootConditionGroup.Id,
            LogicalOperator.Or,
            createdAtUtc);

        rule.AddCondition(
            nestedGroupId,
            ComparisonOperator.GreaterThan,
            50m,
            createdAtUtc);

        return rule;
    }
}
