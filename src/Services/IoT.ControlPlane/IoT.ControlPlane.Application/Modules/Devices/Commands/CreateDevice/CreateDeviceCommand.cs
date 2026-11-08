using MediatR;

namespace Iot.ControlPlane.Application.Modules.Devices.CreateDevice;

public sealed record CreateDeviceCommand(
    Guid SiteId, string Name, string SerialNumber
    ) : IRequest<Guid>;
