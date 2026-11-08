
using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Commands.UpdateDevice;

public sealed record UpdateDeviceCommand(Guid Id, string Name) : IRequest<bool>;
