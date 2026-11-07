using IoT.ControlPlane.Application.Modules.Sites.DTOs;
using IoT.ControlPlane.Domain.Modules.Sites;
using IoT.ControlPlane.Application.Modules.Organizations.Contracts;
using IoT.ControlPlane.Application.Modules.Sites.Abstractions;

namespace IoT.ControlPlane.Application.Modules.Sites;

public sealed class SiteService(
ISiteRepository siteRepository,
IOrganizationExistenceChecker organizationExistenceChecker,
TimeProvider timeProvider
) {
    public async Task<SiteDto> CreateAsync(Guid organizationId, string name, string? address, CancellationToken cancellationToken) {
        var organizationExist = await organizationExistenceChecker.ExistsAsync(organizationId, cancellationToken);
        if (!organizationExist) {
            throw new KeyNotFoundException($"Organization with ID '{organizationId}' was not found.");
        }
        var site = Site.Create(
            Guid.NewGuid(),
            organizationId,
            name,
            address,
            timeProvider.GetUtcNow()
        );
        await siteRepository.AddAsync(site, cancellationToken);
        return MapToDto(site);
    }
    public async Task<SiteDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken) {
        var site = await siteRepository.GetByIdAsync(
            id,
            cancellationToken);

        return site is null ? null : MapToDto(site);
    }

    public async Task<IReadOnlyList<SiteDto>> GetAllAsync(
        CancellationToken cancellationToken) {
        var sites = await siteRepository.GetAllAsync(cancellationToken);

        return sites.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<SiteDto>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken) {
        var sites = await siteRepository.GetByOrganizationIdAsync(
            organizationId,
            cancellationToken);

        return sites.Select(MapToDto).ToList();
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        string name,
        string? address,
        CancellationToken cancellationToken) {
        var site = await siteRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (site is null)
            return false;

        site.UpdateDetails(
            name,
            address,
            timeProvider.GetUtcNow());

        await siteRepository.UpdateAsync(site, cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken) {
        var site = await siteRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (site is null)
            return false;

        await siteRepository.DeleteAsync(site, cancellationToken);

        return true;
    }




    private static SiteDto MapToDto(Site site) {
        return new SiteDto(
            site.Id,
            site.OrganizationId,
            site.Name,
            site.Address,
            site.CreatedAtUtc,
            site.UpdatedAtUtc);
    }

}