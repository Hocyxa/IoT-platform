


using IoT.ControlPlane.Api.Contracts.Organizations;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.CreateOrganization;
using IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizationById;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/organizations")]
public sealed class OrganizationsControler(CreateOrganizationHandler createOrganizationHandler, GetOrganizationByIdHandler getOrganizationByIdHandler) : ControllerBase
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
}