using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRulesByDeviceId;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Features.GetMonitoringRulesByDeviceId;

public sealed class GetMonitoringRulesByDeviceIdQueryHandlerTests
{
    private readonly IMonitoringRuleRepository _repository =
        Substitute.For<IMonitoringRuleRepository>();

    private readonly GetMonitoringRulesByDeviceIdQueryHandler _handler;

    public GetMonitoringRulesByDeviceIdQueryHandlerTests()
    {
        _handler = new GetMonitoringRulesByDeviceIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnRules_WhenRulesExist()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var deviceId = Guid.NewGuid();

        var firstRule = CreateRule(deviceId, "Temperature rule", now);
        var secondRule = CreateRule(deviceId, "Humidity rule", now);

        _repository.GetByDeviceIdAsync(
                deviceId,
                Arg.Any<CancellationToken>())
            .Returns(new[] { firstRule, secondRule });

        var query = new GetMonitoringRulesByDeviceIdQuery(deviceId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Contains(result, rule => rule.Id == firstRule.Id);
        Assert.Contains(result, rule => rule.Id == secondRule.Id);

        Assert.All(result, rule => Assert.Equal(deviceId, rule.DeviceId));

        await _repository.Received(1).GetByDeviceIdAsync(
            deviceId,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyCollection_WhenNoRulesExist()
    {
        // Arrange
        var deviceId = Guid.NewGuid();

        _repository.GetByDeviceIdAsync(
                deviceId,
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<MonitoringRule>());

        var query = new GetMonitoringRulesByDeviceIdQuery(deviceId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);

        await _repository.Received(1).GetByDeviceIdAsync(
            deviceId,
            Arg.Any<CancellationToken>());
    }

    private static MonitoringRule CreateRule(
        Guid deviceId,
        string name,
        DateTimeOffset createdAtUtc)
    {
        return MonitoringRule.Create(
            deviceId,
            name,
            null,
            RuleSeverity.Warning,
            createdAtUtc);
    }
}
