


using IoT.ControlPlane.Api.Contracts.Organizations;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.CreateOrganization;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.DeleteOrganization;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.RenameOrganization;
using IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizationById;
using IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizations;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

[ApiController]
[Route("api/v1/[controller]")]
// [ApiVersion("1.0")] // Version 1.0
public sealed class OrganizationsController(CreateOrganizationHandler createOrganizationHandler, GetOrganizationByIdHandler getOrganizationByIdHandler,
    GetOrganizationsHandler getOrganizationsHandler, RenameOrganizationHandler renameOrganizationHandler, DeleteOrganizationHandler deleteOrganizationHandler) : ControllerBase
{
    ///
    [HttpPost]
    [ProducesResponseType<OrganizationResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrganizationCommand(request.Name);
        var organization = await createOrganizationHandler.HandleAsync(command, cancellationToken);
        var response = new OrganizationResponse(organization.Id,
            organization.Name,
            organization.CreatedAtUtc);
        return CreatedAtAction(
            nameof(GetById),
            new { id = organization.Id },
            response);

    }
    [HttpGet("{id:guid}")]
    [ProducesResponseType<OrganizationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrganizationByIdQuery(id);

        var organization = await getOrganizationByIdHandler.HandleAsync(query, cancellationToken);
        if (organization is null)
        {
            return NotFound();
        }

        var response = new OrganizationResponse(organization.Id, organization.Name, organization.CreatedAtUtc);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<OrganizationResponse>>(
    StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
    CancellationToken cancellationToken)
    {
        var query = new GetOrganizationsQuery();

        var organizations = await getOrganizationsHandler.HandleAsync(
            query,
            cancellationToken);

        var response = organizations
            .Select(organization => new OrganizationResponse(
                organization.Id,
                organization.Name,
                organization.CreatedAtUtc))
            .ToList();

        return Ok(response);
    }
    [HttpPatch("{id:guid}")]
    [ProducesResponseType<OrganizationResponse>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename(Guid id, [FromBody] RenameOrganizationRequest request, CancellationToken cancellationToken)
    {
        var query = new RenameOrganizationCommand(id, request.Name);

        var rename = await renameOrganizationHandler.HandleAsync(query, cancellationToken);
        if (!rename)
        {
            return NotFound();
        }

        return NoContent();
    }
    [HttpDelete("{id:guid}")]
    [ProducesResponseType<OrganizationResponse>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var query = new DeleteOrganizationCommand(id);

        var delete = await deleteOrganizationHandler.HandleAsync(query, cancellationToken);
        if (!delete)
        {
            return NotFound();
        }

        return NoContent();
    }

}