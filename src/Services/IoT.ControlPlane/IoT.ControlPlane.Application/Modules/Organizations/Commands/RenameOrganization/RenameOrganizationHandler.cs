
using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;

namespace IoT.ControlPlane.Application.Modules.Organizations.Commands.RenameOrganization;

public sealed class RenameOrganizationHandler(
    IOrganizationRepository repository, TimeProvider timeProvider)
{
    public async Task<bool> HandleAsync(
        RenameOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await repository.GetByIdAsync(command.OrganizationId, cancellationToken);

        if (organization is null)
        {
            return false;
        }

        organization.Rename(command.Name, timeProvider.GetUtcNow());

        await repository.UpdateAsync(
            organization,
            cancellationToken);


        return true;
    }
}
