namespace IoT.ControlPlane.Api.E2E.Tests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class DockerComposeCollection
    : ICollectionFixture<DockerComposeFixture>
{
    public const string Name = "Docker Compose E2E";
}
