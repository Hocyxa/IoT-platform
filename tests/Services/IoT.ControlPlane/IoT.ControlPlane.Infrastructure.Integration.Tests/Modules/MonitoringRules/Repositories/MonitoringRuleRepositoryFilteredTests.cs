using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Sensors;
using IoT.ControlPlane.Infrastructure.Integration.Tests.Fixtures;
using IoT.ControlPlane.Infrastructure.Persistence.Repositories;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.Modules.MonitoringRules.Repositories;

public sealed class MonitoringRuleRepositoryTests
    : IClassFixture<MonitoringRuleRepositoryFixture>
{
    private readonly MonitoringRuleRepositoryFixture _fixture;

    public MonitoringRuleRepositoryTests(
        MonitoringRuleRepositoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnPaginatedRules()
    {
        // Arrange
        await using var dbContext = _fixture.CreateDbContext();


        var expectedRules = _fixture.Rules;

        var repository = new MonitoringRuleRepository(dbContext);

        // Act
        var (items, totalCount) = await repository.GetFilteredAsync(
            deviceId: null,
            severity: null,
            isEnabled: null,
            measurementType: null,
            page: 1,
            pageSize: 2,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(expectedRules.Count, totalCount);
        Assert.Equal(2, items.Count);

        Assert.Equal(
            expectedRules
                .OrderByDescending(x => x.CreatedAtUtc)
                .ThenBy(x => x.Id)
                .Take(2)
                .Select(x => x.Id),
            items.Select(x => x.Id));

        Assert.All(items, rule =>
            Assert.NotNull(rule.RootConditionGroup));
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldFilterBySeverity()
    {
        await using var dbContext = _fixture.CreateDbContext();

        var repository = new MonitoringRuleRepository(dbContext);

        var expectedSeverity = RuleSeverity.Warning;

        var expectedRules = _fixture.Rules
            .Where(x => x.Severity == expectedSeverity)
            .ToList();

        var (items, totalCount) = await repository.GetFilteredAsync(
            deviceId: null,
            severity: expectedSeverity,
            isEnabled: null,
            measurementType: null,
            page: 1,
            pageSize: 20,
            cancellationToken: CancellationToken.None);

        Assert.Equal(expectedRules.Count, totalCount);
        Assert.Equal(expectedRules.Count, items.Count);

        Assert.All(items, rule =>
            Assert.Equal(expectedSeverity, rule.Severity));

        Assert.Equal(
            expectedRules.Select(x => x.Id).OrderBy(x => x),
            items.Select(x => x.Id).OrderBy(x => x));
    }
    [Fact]
    public async Task GetFilteredAsync_ShouldFilterByDeviceId()
    {
        // Arrange
        await using var dbContext = _fixture.CreateDbContext();

        var expectedDeviceId = _fixture.Rules.First().DeviceId;

        var expectedRules = _fixture.Rules
            .Where(x => x.DeviceId == expectedDeviceId)
            .ToList();

        var repository = new MonitoringRuleRepository(dbContext);

        // Act
        var (items, totalCount) = await repository.GetFilteredAsync(
            deviceId: expectedDeviceId,
            severity: null,
            isEnabled: null,
            measurementType: null,
            page: 1,
            pageSize: 20,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(expectedRules.Count, totalCount);
        Assert.Equal(expectedRules.Count, items.Count);

        Assert.All(items, rule =>
            Assert.Equal(expectedDeviceId, rule.DeviceId));

        Assert.Equal(
            expectedRules.Select(x => x.Id).OrderBy(x => x),
            items.Select(x => x.Id).OrderBy(x => x));
    }
    [Fact]
    public async Task GetFilteredAsync_ShouldFilterByIsEnabled()
    {
        // Arrange
        await using var dbContext = _fixture.CreateDbContext();

        const bool expectedIsEnabled = true;

        var expectedRules = _fixture.Rules
            .Where(x => x.IsEnabled == expectedIsEnabled)
            .ToList();

        var repository = new MonitoringRuleRepository(dbContext);

        // Act
        var (items, totalCount) = await repository.GetFilteredAsync(
            deviceId: null,
            severity: null,
            isEnabled: expectedIsEnabled,
            measurementType: null,
            page: 1,
            pageSize: 20,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(expectedRules.Count, totalCount);
        Assert.Equal(expectedRules.Count, items.Count);

        Assert.All(items, rule =>
            Assert.True(rule.IsEnabled));
    }
    [Fact]
    public async Task GetFilteredAsync_ShouldFilterByMeasurementType()
    {
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new MonitoringRuleRepository(dbContext);

        var expectedRule = _fixture.Rules.Skip(1).First();

        var (items, totalCount) = await repository.GetFilteredAsync(
            deviceId: null,
            severity: null,
            isEnabled: null,
            measurementType: MeasurementType.Humidity,
            page: 1,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        Assert.Equal(1, totalCount);
        var rule = Assert.Single(items);
        Assert.Equal(expectedRule.Id, rule.Id);
    }
    [Fact]
    public async Task GetFilteredAsync_ShouldApplyMultipleFilters()
    {
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new MonitoringRuleRepository(dbContext);

        var expectedRules = _fixture.Rules
            .Where(x =>
                x.Severity == RuleSeverity.Warning &&
                !x.IsEnabled)
            .ToList();

        var (items, totalCount) = await repository.GetFilteredAsync(
            deviceId: null,
            severity: RuleSeverity.Warning,
            isEnabled: false,
            measurementType: null,
            page: 1,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        Assert.Equal(expectedRules.Count, totalCount);
        Assert.Equal(
            expectedRules.Select(x => x.Id).OrderBy(x => x),
            items.Select(x => x.Id).OrderBy(x => x));
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnEmptyResultWhenNoRulesMatch()
    {
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new MonitoringRuleRepository(dbContext);

        var (items, totalCount) = await repository.GetFilteredAsync(
            deviceId: Guid.NewGuid(),
            severity: null,
            isEnabled: null,
            measurementType: null,
            page: 1,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        Assert.Empty(items);
        Assert.Equal(0, totalCount);
    }
    [Fact]
    public async Task GetFilteredAsync_ShouldReturnTotalCountRegardlessOfPageSize()
    {
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new MonitoringRuleRepository(dbContext);

        const int pageSize = 2;

        var (firstPage, firstTotalCount) = await repository.GetFilteredAsync(
            deviceId: null,
            severity: null,
            isEnabled: null,
            measurementType: null,
            page: 1,
            pageSize: pageSize,
            cancellationToken: CancellationToken.None);

        var (secondPage, secondTotalCount) = await repository.GetFilteredAsync(
            deviceId: null,
            severity: null,
            isEnabled: null,
            measurementType: null,
            page: 2,
            pageSize: pageSize,
            cancellationToken: CancellationToken.None);

        Assert.Equal(_fixture.Rules.Count, firstTotalCount);
        Assert.Equal(firstTotalCount, secondTotalCount);

        Assert.Equal(pageSize, firstPage.Count);
        Assert.Equal(pageSize, secondPage.Count);

        Assert.Empty(
            firstPage.Select(x => x.Id)
                .Intersect(secondPage.Select(x => x.Id)));
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldLoadAssignedSensors()
    {
        // Arrange
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new MonitoringRuleRepository(dbContext);

        var expectedRule = _fixture.Rules[0];

        var expectedSensor = _fixture.Sensors.Single(
            x => x.DeviceId == expectedRule.DeviceId
                 && x.Type == MeasurementType.Temperature);

        // Act
        var (items, totalCount) = await repository.GetFilteredAsync(
            deviceId: expectedRule.DeviceId,
            severity: null,
            isEnabled: null,
            measurementType: null,
            page: 1,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        // Assert
        var actualRule = Assert.Single(
            items,
            x => x.Id == expectedRule.Id);

        var actualSensor = Assert.Single(actualRule.Sensors);

        Assert.Equal(_fixture.Rules.Count(x => x.DeviceId == expectedRule.DeviceId), totalCount);
        Assert.Equal(expectedSensor.Id, actualSensor.SensorId);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldHandlePaginationBoundaries()
    {
        // Arrange
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new MonitoringRuleRepository(dbContext);

        var expectedTotalCount = _fixture.Rules.Count;

        // Act 1: pageSize = 1
        var (singleItem, singleItemTotalCount) =
            await repository.GetFilteredAsync(
                deviceId: null,
                severity: null,
                isEnabled: null,
                measurementType: null,
                page: 1,
                pageSize: 1,
                cancellationToken: CancellationToken.None);

        // Act 2: последняя страница
        var (lastPage, lastPageTotalCount) =
            await repository.GetFilteredAsync(
                deviceId: null,
                severity: null,
                isEnabled: null,
                measurementType: null,
                page: 2,
                pageSize: 3,
                cancellationToken: CancellationToken.None);

        // Act 3: страница за пределами выборки
        var (emptyPage, emptyPageTotalCount) =
            await repository.GetFilteredAsync(
                deviceId: null,
                severity: null,
                isEnabled: null,
                measurementType: null,
                page: 3,
                pageSize: 2,
                cancellationToken: CancellationToken.None);

        // Assert 1
        Assert.Single(singleItem);
        Assert.Equal(expectedTotalCount, singleItemTotalCount);

        // Assert 2
        Assert.Single(lastPage);
        Assert.Equal(expectedTotalCount, lastPageTotalCount);

        // Assert 3
        Assert.Empty(emptyPage);
        Assert.Equal(expectedTotalCount, emptyPageTotalCount);
    }

}
