using IoT.ControlPlane.Application.Modules.Sensors.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.GetSensors;

public sealed record GetSensorsQuery() : IRequest<IReadOnlyList<SensorDto>>;
