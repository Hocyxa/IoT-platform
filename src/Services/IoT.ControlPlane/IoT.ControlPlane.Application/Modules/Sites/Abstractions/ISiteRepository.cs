
using IoT.ControlPlane.Domain.Modules.Sites;

namespace IoT.ControlPlane.Application.Modules.Sites.Abstractions;

public interface ISiteRepository
{
    Task AddAsync(
        Site site,
        CancellationToken cancellationToken);

    Task<Site?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Site>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Site>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Site site,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Site site,
        CancellationToken cancellationToken);
}
