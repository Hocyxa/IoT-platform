
using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.DeleteOrganization;
using IoT.ControlPlane.Domain.Modules.Organizations;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Organizations.Commands;

public sealed class DeleteOrganizationHandlerTests
{
    private readonly IOrganizationRepository _repository =
        Substitute.For<IOrganizationRepository>();
    private DeleteOrganizationHandler CreateHandler()
    {
        return new DeleteOrganizationHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingOrganization_DeleteOrganization()
    {
        // Arrange
        var organization = CreateOrganization();

        _repository
            .GetByIdAsync(
                organization.Id,
                Arg.Any<CancellationToken>())
            .Returns(organization);

        var handler = CreateHandler();
        var command = new DeleteOrganizationCommand(
            organization.Id);

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result);

        await _repository.Received(1)
            .GetByIdAsync(
                organization.Id,
                Arg.Any<CancellationToken>());

        await _repository.Received(1)
            .DeleteAsync(
                organization,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingOrganization_ReturnsFalse()
    {
        // Arrange
        var organizationId = new Guid();

        _repository
            .GetByIdAsync(
                organizationId,
                Arg.Any<CancellationToken>())
            .Returns((Organization?)null);

        var handler = CreateHandler();
        var command = new DeleteOrganizationCommand(
            organizationId);

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        Assert.False(result);

        await _repository.Received(1)
            .GetByIdAsync(
                organizationId,
                Arg.Any<CancellationToken>());

        await _repository.DidNotReceive().DeleteAsync(
            Arg.Any<Organization>(),
            Arg.Any<CancellationToken>());
    }

    private static Organization CreateOrganization()
    {
        return Organization.Create(
            Guid.NewGuid(),
            "Original organization",
            DateTimeOffset.UtcNow);
    }
}
