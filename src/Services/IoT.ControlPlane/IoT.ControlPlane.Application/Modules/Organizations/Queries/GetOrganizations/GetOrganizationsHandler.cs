

using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
namespace IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizations;

public sealed class GetOrganizationsHandler(IOrganizationRepository repository)
{
    public async Task<IReadOnlyList<OrganizationDto>> HandleAsync(GetOrganizationsQuery query, CancellationToken cancellationToken)
    {
        var organizations = await repository.GetAllAsync(cancellationToken);

        return organizations.Select(org => new OrganizationDto(org.Id, org.Name, org.CreatedAtUtc, org.UpdatedAtUtc)).ToList();
    }
}