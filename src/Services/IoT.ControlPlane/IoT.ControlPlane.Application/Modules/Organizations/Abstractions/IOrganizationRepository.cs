
using IoT.ControlPlane.Domain.Modules.Organizations;

namespace IoT.ControlPlane.Application.Modules.Organizations.Abstractions;

public interface IOrganizationRepository
{
    Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken);

    Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Organization>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
    Task UpdateAsync(
        Organization organization,
        CancellationToken cancellationToken);
    Task DeleteAsync(
         Organization organization,
        CancellationToken cancellationToken);
}
