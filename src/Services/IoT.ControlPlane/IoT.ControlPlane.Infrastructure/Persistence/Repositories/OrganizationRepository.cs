using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Application.Modules.Sites.Contracts;
using IoT.ControlPlane.Domain.Modules.Organizations;

using Microsoft.EntityFrameworkCore;

namespace IoT.ControlPlane.Infrastructure.Persistence.Repositories;

public sealed class OrganizationRepository(
    ControlPlaneDbContext dbContext) : IOrganizationRepository, IOrganizationExistenceChecker
{
    public async Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken)
    {
        await dbContext.Organizations.AddAsync(
            organization,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return dbContext.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                organization => organization.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Organization>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.Organizations
            .AsNoTracking()
            .OrderBy(organization => organization.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return dbContext.Organizations
            .AnyAsync(
                organization => organization.Id == id,
                cancellationToken);
    }

    public async Task UpdateAsync(Organization organization, CancellationToken cancellationToken)
    {
        dbContext.Organizations.Update(organization);

        await dbContext.SaveChangesAsync(cancellationToken);

    }

    public async Task DeleteAsync(Organization organization, CancellationToken cancellationToken)
    {
        dbContext.Remove(organization);
        await dbContext.SaveChangesAsync(cancellationToken);

    }
}
