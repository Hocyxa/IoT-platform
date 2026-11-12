using IoT.ControlPlane.Api.Contracts.MonitoringRules.Mappers;
using IoT.ControlPlane.Api.Contracts.MonitoringRules.Requests;
using IoT.ControlPlane.Api.Contracts.Sensors;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.AddCondition;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.AssignSensorToMonitoringRule;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.CreateMonitoringRule;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.DeleteMonitoringRule;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.RemoveSensorFromMonitoringRule;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.UpdateCondition;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Commands.UpdateMonitoringRule;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.ChangeGroupLogicalOperator;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.DisableMonitoringRule;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.EnableMonitoringRule;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRuleById;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRules;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.GetMonitoringRulesByDeviceId;
using IoT.ControlPlane.Application.Modules.MonitoringRules.Features.RemoveCondition;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace IoT.ControlPlane.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class MonitoringRulesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateMonitoringRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateMonitoringRuleCommand(
            request.DeviceId,
            request.Name,
            request.Description,
            RuleSeverityMapper.ToDomain(request.Severity));

        var id = await sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new { id });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetMonitoringRulesRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetMonitoringRulesQuery(
            DeviceId: request.DeviceId,
            Severity: request.Severity is null
                ? null
                : RuleSeverityMapper.ToDomain(request.Severity.Value),
            IsEnabled: request.IsEnabled,
            MeasurementType: request.MeasurementType is null
                ? null
                : MeasurementTypeMapper.ToDomain(request.MeasurementType.Value),
            Page: request.Page,
            PageSize: request.PageSize);

        var result = await sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetMonitoringRuleByIdQuery(id);

        var rule = await sender.Send(query, cancellationToken);

        if (rule is null)
        {
            return NotFound();
        }

        return Ok(rule);
    }

    [HttpGet("/api/v1/devices/{deviceId:guid}/monitoringrules")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDevice(
        Guid deviceId,
        CancellationToken cancellationToken)
    {
        var query = new GetMonitoringRulesByDeviceIdQuery(deviceId);

        var rules = await sender.Send(query, cancellationToken);

        return Ok(rules);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateMonitoringRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMonitoringRuleCommand(
            id,
            request.Name,
            request.Description,
            RuleSeverityMapper.ToDomain(request.Severity));

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteMonitoringRuleCommand(id);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/enable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Enable(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new EnableMonitoringRuleCommand(id);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/disable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Disable(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DisableMonitoringRuleCommand(id);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/sensors/{sensorId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignSensor(
        Guid id,
        Guid sensorId,
        CancellationToken cancellationToken)
    {
        var command = new AssignSensorToMonitoringRuleCommand(id, sensorId);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}/sensors/{sensorId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveSensor(
        Guid id,
        Guid sensorId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveSensorFromMonitoringRuleCommand(id, sensorId);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/condition-groups/{groupId:guid}/conditions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddCondition(
        Guid id,
        Guid groupId,
        [FromBody] AddConditionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddConditionCommand(
            id,
            groupId,
            ComparisonOperatorMapper.ToDomain(request.ComparisonOperator),
            request.Threshold);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPut("{id:guid}/conditions/{conditionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateCondition(
        Guid id,
        Guid conditionId,
        [FromBody] UpdateConditionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateConditionCommand(
            id,
            conditionId,
            ComparisonOperatorMapper.ToDomain(request.ComparisonOperator),
            request.Threshold);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}/conditions/{conditionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveCondition(
        Guid id,
        Guid conditionId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveConditionCommand(id, conditionId);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/condition-groups/{groupId:guid}/logical-operator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangeGroupLogicalOperator(
        Guid id,
        Guid groupId,
        [FromBody] ChangeGroupLogicalOperatorRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeGroupLogicalOperatorCommand(
            id,
            groupId,
            LogicalOperatorMapper.ToDomain(request.LogicalOperator));

        await sender.Send(command, cancellationToken);

        return NoContent();
    }
}
