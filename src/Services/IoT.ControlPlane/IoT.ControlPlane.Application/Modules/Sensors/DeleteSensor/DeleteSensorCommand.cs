using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.DeleteSensor;

public sealed record DeleteSensorCommand(Guid Id) : IRequest<bool>;
