
using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizations;
using IoT.ControlPlane.Domain.Modules.Organizations;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Organizations.Queries;

public sealed class GetOrganizationsHandlerTests
{
    private readonly IOrganizationRepository _repository =
        Substitute.For<IOrganizationRepository>();

    private GetOrganizationsHandler CreateHandler()
    {
        return new GetOrganizationsHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WhenOrganizationsExist_ReturnsOrganizationDtos()
    {
        // Arrange
        var organizations = new List<Organization>
        {
            CreateOrganization("Organization 1"),
            CreateOrganization("Organization 2"),
        };

        _repository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(organizations);

        var handler = CreateHandler();
        var query = new GetOrganizationsQuery();

        // Act
        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        // Assert
        Assert.Equal(organizations.Count, result.Count);

        for (var i = 0; i < organizations.Count; i++)
        {
            Assert.Equal(organizations[i].Id, result[i].Id);
            Assert.Equal(organizations[i].Name, result[i].Name);
            Assert.Equal(
                organizations[i].CreatedAtUtc,
                result[i].CreatedAtUtc);
        }

        await _repository.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenNoOrganizationsExist_ReturnsEmptyList()
    {
        // Arrange
        _repository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Organization>());

        var handler = CreateHandler();
        var query = new GetOrganizationsQuery();

        // Act
        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        // Assert
        Assert.Empty(result);

        await _repository.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    private static Organization CreateOrganization(string name)
    {
        return Organization.Create(
            Guid.NewGuid(),
            name,
            DateTimeOffset.UtcNow);
    }
}
