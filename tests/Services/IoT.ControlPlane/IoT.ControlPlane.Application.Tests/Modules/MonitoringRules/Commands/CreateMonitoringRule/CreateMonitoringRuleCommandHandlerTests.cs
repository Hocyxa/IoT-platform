
using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.CreateMonitoringRule;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Contracts;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Commands.CreateMonitoringRule;

public sealed class CreateMonitoringRuleCommandHandlerTests
{
    private readonly IMonitoringRuleRepository _monitoringRuleRepository;
    private readonly IDeviceExistenceChecker _deviceExistenceChecker;
    private readonly TimeProvider _timeProvider;

    private readonly CreateMonitoringRuleCommandHandler _handler;

    public CreateMonitoringRuleCommandHandlerTests()
    {
        _monitoringRuleRepository =
            Substitute.For<IMonitoringRuleRepository>();

        _deviceExistenceChecker =
            Substitute.For<IDeviceExistenceChecker>();

        _timeProvider = Substitute.For<TimeProvider>();

        _handler = new CreateMonitoringRuleCommandHandler(
            _monitoringRuleRepository,
            _deviceExistenceChecker,
            _timeProvider);
    }

    [Fact]
    public async Task Handle_WhenDeviceExists_CreatesAndSavesMonitoringRule()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var now = new DateTimeOffset(
            2026, 11, 10, 12, 0, 0, TimeSpan.Zero);

        var command = new CreateMonitoringRuleCommand(
            deviceId,
            "Temperature monitoring",
            "Temperature threshold monitoring",
            RuleSeverity.Warning);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _deviceExistenceChecker
            .ExistsAsync(deviceId, cancellationToken)
            .Returns(true);

        _timeProvider
            .GetUtcNow()
            .Returns(now);

        // Act
        var result = await _handler.Handle(
            command,
            cancellationToken);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        await _deviceExistenceChecker
            .Received(1)
            .ExistsAsync(deviceId, cancellationToken);

        await _monitoringRuleRepository
            .Received(1)
            .AddAsync(
                Arg.Is<MonitoringRule>(rule =>
                    rule.Id == result &&
                    rule.DeviceId == deviceId &&
                    rule.Name == command.Name &&
                    rule.Description == command.Description &&
                    rule.Severity == command.Severity &&
                    !rule.IsEnabled &&
                    rule.CreatedAtUtc == now &&
                    rule.UpdatedAtUtc == now),
                cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenDeviceDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var deviceId = Guid.NewGuid();

        var command = new CreateMonitoringRuleCommand(
            deviceId,
            "Temperature monitoring",
            null,
            RuleSeverity.Warning);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _deviceExistenceChecker
            .ExistsAsync(deviceId, cancellationToken)
            .Returns(false);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        // Assert
        Assert.Contains(deviceId.ToString(), exception.Message);

        await _monitoringRuleRepository
            .DidNotReceive()
            .AddAsync(
                Arg.Any<MonitoringRule>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDeviceExistenceCheckFails_PropagatesException()
    {
        // Arrange
        var deviceId = Guid.NewGuid();

        var command = new CreateMonitoringRuleCommand(
            deviceId,
            "Temperature monitoring",
            null,
            RuleSeverity.Warning);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var expectedException = new InvalidOperationException(
            "Device existence check failed.");

        _deviceExistenceChecker
            .ExistsAsync(deviceId, cancellationToken)
            .Returns<bool>(_ => throw expectedException);

        // Act
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        // Assert
        Assert.Same(expectedException, actualException);

        await _monitoringRuleRepository
            .DidNotReceive()
            .AddAsync(
                Arg.Any<MonitoringRule>(),
                Arg.Any<CancellationToken>());
    }
}
