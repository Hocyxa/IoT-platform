
using IoT.ControlPlane.Application.Modules.Devices.DTOs;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Queries.GetDevicesBySite;

public sealed record GetDevicesBySiteQuery(Guid SiteId) : IRequest<IReadOnlyList<DeviceDto>>;
