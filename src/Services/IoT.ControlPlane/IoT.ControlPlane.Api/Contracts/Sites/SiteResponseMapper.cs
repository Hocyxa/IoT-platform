using IoT.ControlPlane.Application.Modules.Sites.DTOs;

namespace IoT.ControlPlane.Api.Contracts.Sites;

public static class SiteResponseMapper
{
    public static SiteResponse ToResponse(SiteDto site)
    {
        return new SiteResponse(
            site.Id,
            site.OrganizationId,
            site.Name,
            site.Address,
            site.CreatedAtUtc,
            site.UpdatedAtUtc);
    }
}
