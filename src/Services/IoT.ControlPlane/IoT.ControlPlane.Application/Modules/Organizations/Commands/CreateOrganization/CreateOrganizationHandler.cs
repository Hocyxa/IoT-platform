
using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Domain.Modules.Organizations;

namespace IoT.ControlPlane.Application.Modules.Organizations.Commands.CreateOrganization;

public sealed class CreateOrganizationHandler(
    IOrganizationRepository repository,
    TimeProvider timeProvider)
{
    public async Task<Guid> HandleAsync(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = Organization.Create(
            Guid.NewGuid(),
            command.Name,
            timeProvider.GetUtcNow());

        await repository.AddAsync(
            organization,
            cancellationToken);

        return organization.Id;
    }
}
