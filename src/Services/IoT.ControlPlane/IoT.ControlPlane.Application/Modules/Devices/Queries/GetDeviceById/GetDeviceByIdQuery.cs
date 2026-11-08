
using IoT.ControlPlane.Application.Modules.Devices.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Queries.GetDeviceById;

public sealed record GetDeviceByIdQuery(Guid Id) : IRequest<DeviceDto?>;
