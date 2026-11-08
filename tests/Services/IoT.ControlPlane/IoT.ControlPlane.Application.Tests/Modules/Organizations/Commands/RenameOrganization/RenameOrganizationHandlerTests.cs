
using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.RenameOrganization;
using IoT.ControlPlane.Domain.Modules.Organizations;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Organizations.Commands;

public sealed class RenameOrganizationHandlerTests
{
    private readonly IOrganizationRepository _repository =
        Substitute.For<IOrganizationRepository>();
    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();
    private RenameOrganizationHandler CreateHandler()
    {
        return new RenameOrganizationHandler(_repository, _timeProvider);
    }
    private static readonly DateTimeOffset _createdAtUtc =
            new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);
    [Fact]
    public async Task HandleAsync_WhenOrganizationExists_RenamesOrganization()
    {
        // Arrange
        var organization = CreateOrganization();

        var updatedAtUtc = _createdAtUtc.AddHours(1);

        _repository
            .GetByIdAsync(
                organization.Id,
                Arg.Any<CancellationToken>())
            .Returns(organization);

        _timeProvider
            .GetUtcNow()
            .Returns(updatedAtUtc);

        var handler = CreateHandler();
        var command = new RenameOrganizationCommand(
            organization.Id,
            "Updated organization");

        // Act
        var result = await handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal("Updated organization", organization.Name);
        Assert.Equal(updatedAtUtc, organization.UpdatedAtUtc);

        await _repository.Received(1)
            .GetByIdAsync(
                organization.Id,
                Arg.Any<CancellationToken>());

        await _repository.Received(1)
            .UpdateAsync(
                Arg.Is<Organization>(updated =>
                    updated.Id == organization.Id &&
                    updated.Name == "Updated organization"),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenOrganizationDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var organizationId = Guid.NewGuid();

        _repository
            .GetByIdAsync(
                organizationId,
                Arg.Any<CancellationToken>())
            .Returns((Organization?)null);

        var handler = CreateHandler();
        var command = new RenameOrganizationCommand(
            organizationId,
            "Updated organization");

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

        await _repository.DidNotReceive()
            .UpdateAsync(
                Arg.Any<Organization>(),
                Arg.Any<CancellationToken>());
    }

    private static Organization CreateOrganization()
    {
        return Organization.Create(
            Guid.NewGuid(),
            "Original organization",
            _createdAtUtc);
    }
}
