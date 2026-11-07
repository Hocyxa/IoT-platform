
using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Application.Modules.Organizations.Queries;
using IoT.ControlPlane.Domain.Modules.Organizations;

namespace IoT.ControlPlane.Application.Modules.Organizations.Commands.CreateOrganization;

public sealed class CreateOrganizationHandler(
    IOrganizationRepository repository,
    TimeProvider timeProvider)
{
    public async Task<OrganizationDto> HandleAsync(
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

        return new OrganizationDto(organization.Id, organization.Name, organization.CreatedAtUtc, organization.UpdatedAtUtc);
    }
}
