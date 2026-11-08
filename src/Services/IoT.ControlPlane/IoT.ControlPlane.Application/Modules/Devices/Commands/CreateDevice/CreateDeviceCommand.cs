using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Commands.CreateDevice;

public sealed record CreateDeviceCommand(
    Guid SiteId, string Name, string SerialNumber
    ) : IRequest<Guid>;
