

using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
namespace IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizationById;

public sealed class GetOrganizationByIdHandler(IOrganizationRepository repository)
{
    public async Task<OrganizationDto?> HandleAsync(GetOrganizationByIdQuery query, CancellationToken cancellationToken)
    {
        var organization = await repository.GetByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return null;
        }

        return new OrganizationDto(organization.Id, organization.Name, organization.CreatedAtUtc);
    }
}