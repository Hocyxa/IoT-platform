
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.RemoveSensorFromMonitoringRule;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Commands.RemoveSensorFromMonitoringRule;

public sealed class RemoveSensorFromMonitoringRuleCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _monitoringRuleRepository;
    private readonly TimeProvider _timeProvider;

    private readonly RemoveSensorFromMonitoringRuleCommandHandler _handler;

    public RemoveSensorFromMonitoringRuleCommandHandlerTests()
    {
        _monitoringRuleRepository =
            Substitute.For<IMonitoringRuleRepository>();

        _timeProvider = Substitute.For<TimeProvider>();

        _handler = new RemoveSensorFromMonitoringRuleCommandHandler(
            _monitoringRuleRepository,
            _timeProvider);
    }

    private static MonitoringRule CreateRuleWithSensor(
        Guid deviceId,
        Guid sensorId)
    {
        var createdAtUtc = new DateTimeOffset(
            2026, 11, 10, 10, 0, 0, TimeSpan.Zero);

        var rule = MonitoringRule.Create(
            deviceId,
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            createdAtUtc);

        rule.AssignSensor(
            sensorId,
            deviceId,
            MeasurementType.Temperature,
            createdAtUtc);

        return rule;
    }

    [Fact]
    public async Task Handle_WhenSensorIsAssigned_RemovesSensorAndSavesRule()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var sensorId = Guid.NewGuid();

        var rule = CreateRuleWithSensor(deviceId, sensorId);

        var command = new RemoveSensorFromMonitoringRuleCommand(
            rule.Id,
            sensorId);

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
        Assert.Empty(rule.Sensors);
        Assert.Null(rule.MeasurementType);
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
        var sensorId = Guid.NewGuid();

        var command = new RemoveSensorFromMonitoringRuleCommand(
            ruleId,
            sensorId);

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
    public async Task Handle_WhenSensorIsNotAssigned_ThrowsInvalidOperationException()
    {
        // Arrange
        var rule = MonitoringRule.Create(
            Guid.NewGuid(),
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            new DateTimeOffset(
                2026, 11, 10, 10, 0, 0, TimeSpan.Zero));

        var sensorId = Guid.NewGuid();

        var command = new RemoveSensorFromMonitoringRuleCommand(
            rule.Id,
            sensorId);

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
