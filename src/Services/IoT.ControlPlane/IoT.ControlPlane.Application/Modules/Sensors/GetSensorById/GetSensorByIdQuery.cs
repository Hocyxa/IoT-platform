using IoT.ControlPlane.Application.Modules.Sensors.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.GetSensorById;

public sealed record GetSensorByIdQuery(Guid Id) : IRequest<SensorDto>;
