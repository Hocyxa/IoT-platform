
using IoT.ControlPlane.Api.E2E.Tests.Clients;
using IoT.ControlPlane.Api.E2E.Tests.Contracts;

namespace IoT.ControlPlane.Api.E2E.Tests.Workflows;

public sealed class OrganizationProvisioningWorkflow
{
    private readonly OrganizationsClient _organizations;
    private readonly SitesClient _sites;
    private readonly DevicesClient _devices;
    private readonly SensorsClient _sensors;
    private readonly MonitoringRulesClient _monitoringRules;

    public OrganizationProvisioningWorkflow(HttpClient httpClient)
    {
        _organizations = new OrganizationsClient(httpClient);
        _sites = new SitesClient(httpClient);
        _devices = new DevicesClient(httpClient);
        _sensors = new SensorsClient(httpClient);
        _monitoringRules = new MonitoringRulesClient(httpClient);
    }

    public async Task<OrganizationProvisioningResult> RunAsync(
        CancellationToken ct = default)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        // 1. Organization
        var organization = await _organizations.CreateAsync(
            new CreateOrganizationRequest(
                $"E2E Organization {suffix}"),
            ct);

        // 2. Site
        var site = await _sites.CreateAsync(
            new CreateSiteRequest(
                organization.Id,
                $"E2E Site {suffix}",
                "Test address"),
            ct);

        // 3. Device
        var device = await _devices.CreateAsync(
            new CreateDeviceRequest(
                site.Id,
                $"E2E Device {suffix}",
                $"E2E-SN-{suffix}"),
            ct);

        // 4. Sensor
        var sensor = await _sensors.CreateAsync(
            new CreateSensorRequest(
                device.Id,
                $"E2E Temperature Sensor {suffix}",
                $"E2E-TEMP-{suffix}",
                MeasurementTypeRequest.Temperature,
                "°C"),
            ct);

        // 5. MonitoringRule
        var ruleId = await _monitoringRules.CreateAsync(
            new CreateMonitoringRuleRequest(
                device.Id,
                $"E2E Temperature Rule {suffix}",
                "Created by E2E test",
                RuleSeverityRequest.Warning),
            ct);

        // 6. Assign sensor to rule
        await _monitoringRules.AssignSensorAsync(
            ruleId,
            sensor.Id,
            ct);

        // 7. Get root condition group
        var ruleBeforeEnable =
            await _monitoringRules.GetByIdAsync(ruleId, ct);

        var rootGroupId = ruleBeforeEnable.RootConditionGroup.Id;

        // 8. Add condition: temperature > 30
        await _monitoringRules.AddConditionAsync(
            ruleId,
            rootGroupId,
            new AddConditionRequest(
                ComparisonOperatorRequest.GreaterThan,
                30m),
            ct);

        // 9. Enable rule
        await _monitoringRules.EnableAsync(ruleId, ct);

        // 10. Read final state
        var enabledRule =
            await _monitoringRules.GetByIdAsync(ruleId, ct);

        return new OrganizationProvisioningResult(
            organization,
            site,
            device,
            sensor,
            enabledRule);
    }
}

public sealed record OrganizationProvisioningResult(
    OrganizationResponse Organization,
    SiteResponse Site,
    DeviceResponse Device,
    SensorResponse Sensor,
    MonitoringRuleDto MonitoringRule);
