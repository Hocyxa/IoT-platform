using IoT.ControlPlane.Domain.Modules.Devices;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.Organizations;
using IoT.ControlPlane.Domain.Modules.Sensors;
using IoT.ControlPlane.Domain.Modules.Sites;
using IoT.ControlPlane.Infrastructure.Integration.Tests.Fixture;
using IoT.ControlPlane.Infrastructure.Integration.Tests.TestData;
using IoT.ControlPlane.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace IoT.ControlPlane.Infrastructure.Integration.Tests.Fixtures;

public sealed class MonitoringRuleRepositoryFixture : IAsyncLifetime
{
    private readonly PostgreSqlFixture _postgresSqlFixture = new();

    private readonly TestTimeProvider _timeProvider =
        new(DateTimeOffset.UtcNow);


    public Organization Organization { get; private set; } = null!;

    public Site Site { get; private set; } = null!;

    public IReadOnlyList<Device> Devices { get; private set; } =
        Array.Empty<Device>();

    public IReadOnlyList<Sensor> Sensors { get; private set; } =
        Array.Empty<Sensor>();

    public IReadOnlyList<MonitoringRule> Rules { get; private set; } =
        Array.Empty<MonitoringRule>();

    public async Task InitializeAsync()
    {
        await _postgresSqlFixture.InitializeAsync();

        await using var dbContext = CreateDbContext();

        await dbContext.Database.MigrateAsync();

        CreateTestData();

        dbContext.Organizations.Add(Organization);
        dbContext.Sites.Add(Site);
        dbContext.Devices.AddRange(Devices);
        dbContext.Sensors.AddRange(Sensors);
        dbContext.MonitoringRules.AddRange(Rules);

        await dbContext.SaveChangesAsync();
    }

    public ControlPlaneDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ControlPlaneDbContext>()
            .UseNpgsql(_postgresSqlFixture.ConnectionString)
            .Options;

        return new ControlPlaneDbContext(options);
    }

    private void CreateTestData()
    {
        Organization = OrganizationTestDataFactory.Create(_timeProvider);

        Site = SiteTestDataFactory.Create(
            _timeProvider,
            Organization.Id);

        var device1 = DeviceTestDataFactory.Create(
            _timeProvider,
            Site.Id, 1);

        var device2 = DeviceTestDataFactory.Create(
            _timeProvider,
            Site.Id, 2);

        Devices = [device1, device2];

        var temperatureSensor1 = SensorTestDataFactory.Create(
            _timeProvider,
            device1.Id,
            MeasurementType.Temperature,
            name: "Temperature sensor 1",
            code: "TEMP-001");

        var humiditySensor1 = SensorTestDataFactory.Create(
            _timeProvider,
            device1.Id,
            MeasurementType.Humidity,
            name: "Humidity sensor 1",
            code: "HUM-001");

        var temperatureSensor2 = SensorTestDataFactory.Create(
            _timeProvider,
            device2.Id,
            MeasurementType.Temperature,
            name: "Temperature sensor 2",
            code: "TEMP-002");

        Sensors =
        [
            temperatureSensor1,
            humiditySensor1,
            temperatureSensor2
        ];

        var rule1 = MonitoringRuleTestDataFactory.Create(
            _timeProvider,
            device1.Id,
            RuleSeverity.Warning,
            index: 1);

        var rule2 = MonitoringRuleTestDataFactory.Create(
            _timeProvider,
            device1.Id,
            RuleSeverity.Critical,
            index: 2);

        var rule3 = MonitoringRuleTestDataFactory.Create(
            _timeProvider,
            device2.Id,
            RuleSeverity.Warning,
            index: 3);

        var rule4 = MonitoringRuleTestDataFactory.Create(
            _timeProvider,
            device2.Id,
            RuleSeverity.Critical,
            index: 4);

        rule1.AssignSensor(
            temperatureSensor1.Id,
            temperatureSensor1.DeviceId,
            temperatureSensor1.Type,
            _timeProvider.GetUtcNow());

        rule1.AddCondition(
            rule1.RootConditionGroup.Id,
            ComparisonOperator.GreaterThan,
            30,
            _timeProvider.GetUtcNow());

        rule1.Enable(_timeProvider.GetUtcNow());

        rule2.AssignSensor(
            humiditySensor1.Id,
            humiditySensor1.DeviceId,
            humiditySensor1.Type,
            _timeProvider.GetUtcNow());

        rule3.AssignSensor(
            temperatureSensor2.Id,
            temperatureSensor2.DeviceId,
            temperatureSensor2.Type,
            _timeProvider.GetUtcNow());

        Rules = [rule1, rule2, rule3, rule4];
    }

    public async Task DisposeAsync()
    {

        await _postgresSqlFixture.DisposeAsync();
    }
}
