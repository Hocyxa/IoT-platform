using IoT.ControlPlane.Application.Modules.MonitoringRules.Abstractions;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Sensors;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.MonitoringRules.Features.GetMonitoringRules;

public sealed class GetMonitoringRulesQueryHandlerTests
{
    private readonly IMonitoringRuleRepository _repository =
        Substitute.For<IMonitoringRuleRepository>();

    private readonly GetMonitoringRulesQueryHandler _handler;

    public GetMonitoringRulesQueryHandlerTests()
    {
        _handler = new GetMonitoringRulesQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedRules()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var deviceId = Guid.NewGuid();

        var rule = MonitoringRule.Create(
            deviceId,
            "Temperature monitoring",
            null,
            RuleSeverity.Warning,
            now);

        var query = new GetMonitoringRulesQuery(
            DeviceId: deviceId,
            Severity: RuleSeverity.Warning,
            IsEnabled: false,
            MeasurementType: MeasurementType.Temperature,
            Page: 2,
            PageSize: 10);

        _repository.GetFilteredAsync(
                deviceId,
                RuleSeverity.Warning,
                false,
                MeasurementType.Temperature,
                2,
                10,
                Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<MonitoringRule>)[rule], 15));

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(rule.Id, result.Items[0].Id);
        Assert.Equal(15, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalPages);

        await _repository.Received(1).GetFilteredAsync(
            deviceId,
            RuleSeverity.Warning,
            false,
            MeasurementType.Temperature,
            2,
            10,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPage_WhenNoRulesExist()
    {
        // Arrange
        var query = new GetMonitoringRulesQuery();

        _repository.GetFilteredAsync(
                null,
                null,
                null,
                null,
                1,
                20,
                Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<MonitoringRule>)[], 0));

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(0, result.TotalPages);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    [InlineData(1, 0)]
    [InlineData(1, -10)]
    public async Task Handle_ShouldThrow_WhenPaginationParametersAreInvalid(
        int page,
        int pageSize)
    {
        // Arrange
        var query = new GetMonitoringRulesQuery(
            Page: page,
            PageSize: pageSize);

        // Act
        var act = async () => await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(act);

        await _repository.DidNotReceive().GetFilteredAsync(
            Arg.Any<Guid?>(),
            Arg.Any<RuleSeverity?>(),
            Arg.Any<bool?>(),
            Arg.Any<MeasurementType?>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>());
    }
}
