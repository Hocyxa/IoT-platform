using IoT.ControlPlane.Application.Modules.Sensors.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.GetSensorsByDevice;

public sealed record GetSensorsByDeviceQuery(Guid DeviceId) : IRequest<IReadOnlyList<SensorDto>>;
