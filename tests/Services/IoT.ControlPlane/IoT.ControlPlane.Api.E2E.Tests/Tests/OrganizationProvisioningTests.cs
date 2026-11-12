
using IoT.ControlPlane.Api.E2E.Tests.Infrastructure;
using IoT.ControlPlane.Api.E2E.Tests.Workflows;

namespace IoT.ControlPlane.Api.E2E.Tests.Tests;

[Collection(DockerComposeCollection.Name)]
public sealed class OrganizationProvisioningTests(
    DockerComposeFixture fixture)
{
    [Fact]
    public async Task Should_Create_Organization_And_Enable_MonitoringRule()
    {
        // Arrange
        var workflow = new OrganizationProvisioningWorkflow(
            fixture.Client);

        // Act
        var result = await workflow.RunAsync();

        // Assert
        Assert.NotEqual(Guid.Empty, result.Organization.Id);
        Assert.NotEqual(Guid.Empty, result.Site.Id);
        Assert.NotEqual(Guid.Empty, result.Device.Id);
        Assert.NotEqual(Guid.Empty, result.Sensor.Id);

        Assert.Equal(
            result.Organization.Id,
            result.Site.OrganizationId);

        Assert.Equal(
            result.Site.Id,
            result.Device.SiteId);

        Assert.Equal(
            result.Device.Id,
            result.Sensor.DeviceId);

        Assert.Equal(
            result.Device.Id,
            result.MonitoringRule.DeviceId);

        Assert.True(result.MonitoringRule.IsEnabled);

        Assert.Contains(
            result.MonitoringRule.Sensors,
            sensor => sensor.SensorId == result.Sensor.Id);

        Assert.Contains(
            result.MonitoringRule.RootConditionGroup.Nodes,
            node => node.Type == Contracts.ConditionNodeType.Condition
        );
    }
}
