
using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizationById;
using IoT.ControlPlane.Domain.Modules.Organizations;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Organizations.Queries;

public sealed class GetOrganizationByIdHandlerTests
{
    private readonly IOrganizationRepository _repository =
        Substitute.For<IOrganizationRepository>();

    private GetOrganizationByIdHandler CreateHandler()
    {
        return new GetOrganizationByIdHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WhenOrganizationExists_ReturnsOrganizationDto()
    {
        // Arrange
        var organization = CreateOrganization();

        _repository
            .GetByIdAsync(
                organization.Id,
                Arg.Any<CancellationToken>())
            .Returns(organization);

        var handler = CreateHandler();
        var query = new GetOrganizationByIdQuery(organization.Id);

        // Act
        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(organization.Id, result.Id);
        Assert.Equal(organization.Name, result.Name);
        Assert.Equal(organization.CreatedAtUtc, result.CreatedAtUtc);

        await _repository.Received(1)
            .GetByIdAsync(
                organization.Id,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenOrganizationDoesNotExist_ReturnsNull()
    {
        // Arrange
        var organizationId = Guid.NewGuid();

        _repository
            .GetByIdAsync(
                organizationId,
                Arg.Any<CancellationToken>())
            .Returns((Organization?)null);

        var handler = CreateHandler();
        var query = new GetOrganizationByIdQuery(organizationId);

        // Act
        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        // Assert
        Assert.Null(result);

        await _repository.Received(1)
            .GetByIdAsync(
                organizationId,
                Arg.Any<CancellationToken>());
    }

    private static Organization CreateOrganization()
    {
        return Organization.Create(
            Guid.NewGuid(),
            "Test organization",
            DateTimeOffset.UtcNow);
    }
}
