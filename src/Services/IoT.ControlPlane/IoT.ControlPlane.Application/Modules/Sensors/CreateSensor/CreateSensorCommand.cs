using IoT.ControlPlane.Domain.Modules.Sensors;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.CreateSensor;

public sealed record CreateSensorCommand(Guid DeviceId,
    string Name,
    string Code,
    MeasurementType MeasurementType,
    string Unit) : IRequest<Guid>;
