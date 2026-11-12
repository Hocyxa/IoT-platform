
using IoT.ControlPlane.Api.Contracts.Devices;
using IoT.ControlPlane.Application.Modules.Devices.Commands.ChangeDeviceStatus;
using IoT.ControlPlane.Application.Modules.Devices.Commands.CreateDevice;
using IoT.ControlPlane.Application.Modules.Devices.Commands.DeleteDevice;
using IoT.ControlPlane.Application.Modules.Devices.Commands.UpdateDevice;
using IoT.ControlPlane.Application.Modules.Devices.Queries.GetDeviceById;
using IoT.ControlPlane.Application.Modules.Devices.Queries.GetDevices;
using IoT.ControlPlane.Application.Modules.Devices.Queries.GetDevicesBySite;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace IoT.ControlPlane.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class DevicesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<DeviceResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CreateDeviceRequest request, CancellationToken cancellationToken)
    {
        var id = await sender.Send(new CreateDeviceCommand(request.SiteId, request.Name, request.SerialNumber), cancellationToken);

        var device = await sender.Send(new GetDeviceByIdQuery(id), cancellationToken);

        var response = DeviceResponseMapper.ToResponse(device!);
        return CreatedAtAction(nameof(GetById), new { id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<DeviceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var device = await sender.Send(new GetDeviceByIdQuery(id), cancellationToken);

        if (device is null)
        {
            return NotFound();
        }

        var response = DeviceResponseMapper.ToResponse(device);
        return Ok(response);
    }
    [HttpGet]
    [ProducesResponseType<DeviceResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var devices = await sender.Send(new GetDevicesQuery(), cancellationToken);

        var response = devices.Select(DeviceResponseMapper.ToResponse).ToList();
        return Ok(response);
    }
    [HttpPatch("{id:guid}")]
    [ProducesResponseType<DeviceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await sender.Send(new UpdateDeviceCommand(id, request.Name), cancellationToken);
        return updated ? NoContent() : NotFound();
    }
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await sender.Send(new DeleteDeviceCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeDeviceStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Status))
        {
            return BadRequest("Invalid device status.");
        }
        var status = DeviceStatusMapper.ToDomain(request.Status);
        var updated = await sender.Send(new ChangeDeviceStatusCommand(id, status), cancellationToken);
        return updated ? NoContent() : NotFound();
    }
    [HttpGet("/api/v1/sites/{siteId:guid}/devices")]
    [ProducesResponseType<IReadOnlyList<DeviceResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySite(
        Guid siteId,
        CancellationToken cancellationToken)
    {
        var devices = await sender.Send(
            new GetDevicesBySiteQuery(siteId),
            cancellationToken);

        var response = devices
            .Select(DeviceResponseMapper.ToResponse)
            .ToList();

        return Ok(response);
    }
}
