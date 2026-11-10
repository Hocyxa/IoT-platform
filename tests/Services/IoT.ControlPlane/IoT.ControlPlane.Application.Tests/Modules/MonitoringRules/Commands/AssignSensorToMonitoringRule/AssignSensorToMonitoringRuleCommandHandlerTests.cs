
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.AssignSensorToMonitoringRule;
using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Commands.AssignSensorToMonitoringRule;

public sealed class AssignSensorToMonitoringRuleCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _monitoringRuleRepository;
    private readonly ISensorRepository _sensorRepository;
    private readonly TimeProvider _timeProvider;

    private readonly AssignSensorToMonitoringRuleCommandHandler _handler;

    public AssignSensorToMonitoringRuleCommandHandlerTests()
    {
        _monitoringRuleRepository =
            Substitute.For<IMonitoringRuleRepository>();

        _sensorRepository =
            Substitute.For<ISensorRepository>();

        _timeProvider = Substitute.For<TimeProvider>();

        _handler = new AssignSensorToMonitoringRuleCommandHandler(
            _monitoringRuleRepository,
            _sensorRepository,
            _timeProvider);
    }

    private static MonitoringRule CreateRule(Guid deviceId)
    {
        return MonitoringRule.Create(
            deviceId,
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            new DateTimeOffset(2026, 11, 10, 10, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public async Task Handle_WhenRuleAndSensorExist_AssignsSensorAndSavesRule()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var sensorId = Guid.NewGuid();

        var rule = CreateRule(deviceId);

        var sensor = Sensor.Create(
            sensorId,
            deviceId,
            "Temperature sensor",
            "TEMP-001",
            MeasurementType.Temperature,
            "°C",
            new DateTimeOffset(2026, 11, 10, 10, 0, 0, TimeSpan.Zero));

        var command = new AssignSensorToMonitoringRuleCommand(
            rule.Id,
            sensorId);

        // Для теста идентификатор должен совпадать с запрашиваемым.
        // Если Sensor.Create самостоятельно генерирует Id,
        // используй реальный sensor.Id в команде.
        command = command with { SensorId = sensor.Id };

        var updatedAtUtc = new DateTimeOffset(
            2026, 11, 10, 11, 0, 0, TimeSpan.Zero);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _monitoringRuleRepository
            .GetByIdAsync(rule.Id, cancellationToken)
            .Returns(rule);

        _sensorRepository
            .GetByIdAsync(sensor.Id, cancellationToken)
            .Returns(sensor);

        _timeProvider
            .GetUtcNow()
            .Returns(updatedAtUtc);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.Contains(rule.Sensors, x => x.SensorId == sensor.Id);

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

        var command = new AssignSensorToMonitoringRuleCommand(
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

        await _sensorRepository
            .DidNotReceive()
            .GetByIdAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>());

        await _monitoringRuleRepository
            .DidNotReceive()
            .UpdateAsync(
                Arg.Any<MonitoringRule>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSensorDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var rule = CreateRule(deviceId);
        var sensorId = Guid.NewGuid();

        var command = new AssignSensorToMonitoringRuleCommand(
            rule.Id,
            sensorId);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _monitoringRuleRepository
            .GetByIdAsync(rule.Id, cancellationToken)
            .Returns(rule);

        _sensorRepository
            .GetByIdAsync(sensorId, cancellationToken)
            .Returns((Sensor?)null);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        // Assert
        Assert.Contains(sensorId.ToString(), exception.Message);

        await _monitoringRuleRepository
            .DidNotReceive()
            .UpdateAsync(
                Arg.Any<MonitoringRule>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSensorBelongsToAnotherDevice_ThrowsInvalidOperationException()
    {
        // Arrange
        var rule = CreateRule(Guid.NewGuid());

        var sensor = Sensor.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Temperature sensor",
            "TEMP-002",
            MeasurementType.Temperature,
            "°C",
            new DateTimeOffset(2026, 11, 10, 10, 0, 0, TimeSpan.Zero));

        var command = new AssignSensorToMonitoringRuleCommand(
            rule.Id,
            sensor.Id);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _monitoringRuleRepository
            .GetByIdAsync(rule.Id, cancellationToken)
            .Returns(rule);

        _sensorRepository
            .GetByIdAsync(sensor.Id, cancellationToken)
            .Returns(sensor);

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
