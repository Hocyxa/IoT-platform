using Asp.Versioning;

using IoT.ControlPlane.Api.Contracts.Sensors;
using IoT.ControlPlane.Application.Modules.Sensors.CreateSensor;
using IoT.ControlPlane.Application.Modules.Sensors.DeleteSensor;
using IoT.ControlPlane.Application.Modules.Sensors.GetSensorById;
using IoT.ControlPlane.Application.Modules.Sensors.GetSensors;
using IoT.ControlPlane.Application.Modules.Sensors.GetSensorsByDevice;
using IoT.ControlPlane.Application.Modules.Sensors.UpdateSensor;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace IoT.ControlPlane.Api.Controllers;

[ApiController]
[Route("ape/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class SensorsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<SensorResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CreateSensorRequest request, CancellationToken cancellationToken)
    {
        var id = await sender.Send(new CreateSensorCommand(
            request.DeviceId,
            request.Name,
            request.Code,
            MeasurementTypeMapper.ToDomain(request.MeasurementType),
            request.Unit),
            cancellationToken);

        var sensor = await sender.Send(new GetSensorByIdQuery(id), cancellationToken);

        var response = SensorResponseMapper.ToResponse(sensor!);
        return CreatedAtAction(nameof(GetById), new { id }, response);
    }
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SensorResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetSensorsQuery();

        var sensors = await sender.Send(query, cancellationToken);

        return Ok(sensors.Select(SensorResponseMapper.ToResponse));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<SensorResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetSensorByIdQuery(id);

        var sensor = await sender.Send(query, cancellationToken);

        if (sensor is null)
        {
            return NotFound();
        }

        return Ok(SensorResponseMapper.ToResponse(sensor));
    }

    [HttpGet("/api/v{version:apiVersion}/devices/{deviceId:guid}/sensors")]
    [ProducesResponseType<IReadOnlyList<SensorResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDevice(
        Guid deviceId,
        CancellationToken cancellationToken)
    {
        var query = new GetSensorsByDeviceQuery(deviceId);

        var sensors = await sender.Send(query, cancellationToken);

        return Ok(sensors.Select(SensorResponseMapper.ToResponse));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSensorRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSensorCommand(
            id,
            request.Name,
            MeasurementTypeMapper.ToDomain(request.MeasurementType),
            request.Unit);

        var updated = await sender.Send(command, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSensorCommand(id);

        var deleted = await sender.Send(command, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
