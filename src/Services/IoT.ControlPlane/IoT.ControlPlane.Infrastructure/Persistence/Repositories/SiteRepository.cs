using IoT.ControlPlane.Application.Modules.Sites.Abstractions;
using IoT.ControlPlane.Application.Modules.Sites.Contracts;
using IoT.ControlPlane.Domain.Modules.Sites;

using Microsoft.EntityFrameworkCore;

namespace IoT.ControlPlane.Infrastructure.Persistence.Repositories;

public sealed class SiteRepository(ControlPlaneDbContext dbContext) : ISiteRepository, ISiteExistenceChecker
{
    public async Task AddAsync(Site site, CancellationToken cancellationToken)
    {
        await dbContext.Sites.AddAsync(site, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Site site, CancellationToken cancellationToken)
    {
        dbContext.Remove(site);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid siteId, CancellationToken cancellationToken)
    {
        return await dbContext.Sites.AnyAsync(site => site.Id == siteId, cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Sites.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Site?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Sites.FirstOrDefaultAsync(site => site.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        return await dbContext.Sites.AsNoTracking().Where(site => site.OrganizationId == organizationId).ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Site site, CancellationToken cancellationToken)
    {
        dbContext.Sites.Update(site);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
