using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;

namespace IoT.ControlPlane.Application.Modules.Organizations.Commands.DeleteOrganization;

public sealed class DeleteOrganizationHandler(
    IOrganizationRepository repository)
{
    public async Task<bool> HandleAsync(
        DeleteOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await repository.GetByIdAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return false;
        }

        await repository.DeleteAsync(
            organization,
            cancellationToken);

        return true;
    }
}
