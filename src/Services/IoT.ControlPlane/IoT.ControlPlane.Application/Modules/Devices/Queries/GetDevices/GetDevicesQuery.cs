
using IoT.ControlPlane.Application.Modules.Devices.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Queries.GetDevices;


public sealed record GetDevicesQuery() : IRequest<IReadOnlyList<DeviceDto>>;
