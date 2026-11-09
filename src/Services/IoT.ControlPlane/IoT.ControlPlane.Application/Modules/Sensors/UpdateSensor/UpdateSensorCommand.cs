using IoT.ControlPlane.Domain.Modules.Sensors;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.UpdateSensor;

public sealed record UpdateSensorCommand(
    Guid Id,
    string Name,
    MeasurementType MeasurementType,
    string Unit
) : IRequest<bool>;
