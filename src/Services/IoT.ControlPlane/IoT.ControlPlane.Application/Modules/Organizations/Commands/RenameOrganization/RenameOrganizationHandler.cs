
using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;

namespace IoT.ControlPlane.Application.Modules.Organizations.Commands.RenameOrganization;

public sealed class RenameOrganizationHandler(
    IOrganizationRepository repository)
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

        organization.Rename(command.Name);

        await repository.UpdateAsync(
            organization,
            cancellationToken);


        return true;
    }
}
