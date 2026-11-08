
using IoT.ControlPlane.Api.Contracts.Sites;
using IoT.ControlPlane.Application.Modules.Sites;
using IoT.ControlPlane.Application.Modules.Sites.DTOs;

using Microsoft.AspNetCore.Mvc;

namespace IoT.ControlPlane.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class SitesController(
    SiteService siteService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<SiteDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSiteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var site = await siteService.CreateAsync(
                request.OrganizationId,
                request.Name,
                request.Address,
                cancellationToken);
            var response = SiteResponseMapper.ToResponse(site);

            return CreatedAtAction(
                nameof(GetById),
                new { id = site.Id },
                response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<SiteDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var site = await siteService.GetByIdAsync(id, cancellationToken);

        return site is null ? NotFound() : Ok(SiteResponseMapper.ToResponse(site));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SiteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var sites = await siteService.GetAllAsync(cancellationToken);

        return Ok(sites
                    .Select(SiteResponseMapper.ToResponse)
                    .ToList()
                );
    }
    //TODO /api/v1/organizations/{organizatioId:guid}/sites
    [HttpGet("by-organization/{organizationId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<SiteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrganization(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var sites = await siteService.GetByOrganizationIdAsync(
            organizationId,
            cancellationToken);

        return Ok(sites
                    .Select(SiteResponseMapper.ToResponse)
                    .ToList()
                );
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSiteRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await siteService.UpdateAsync(
            id,
            request.Name,
            request.Address,
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await siteService.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
