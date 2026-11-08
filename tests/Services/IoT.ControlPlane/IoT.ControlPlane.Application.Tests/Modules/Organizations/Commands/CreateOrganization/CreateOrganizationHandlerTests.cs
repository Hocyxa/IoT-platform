
using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.CreateOrganization;
using IoT.ControlPlane.Domain.Modules.Organizations;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Organizations.Commands;

public sealed class CreateOrganizationHandlerTests
{
    private readonly IOrganizationRepository _repository =
        Substitute.For<IOrganizationRepository>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    private CreateOrganizationHandler CreateHandler()
    {
        return new CreateOrganizationHandler(
            _repository,
            _timeProvider);
    }

    [Fact]
    public async Task HandleAsync_CreatesOrganization()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        _timeProvider.GetUtcNow().Returns(now);

        var handler = CreateHandler();
        var command = new CreateOrganizationCommand("Test organization");

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);

        await _repository.Received(1)
            .AddAsync(
                Arg.Is<Organization>(organization =>
                    organization.Id == result.Id &&
                    organization.Name == "Test organization" &&
                    organization.CreatedAtUtc == now),
                Arg.Any<CancellationToken>());

        _timeProvider.Received(1).GetUtcNow();
    }
}
