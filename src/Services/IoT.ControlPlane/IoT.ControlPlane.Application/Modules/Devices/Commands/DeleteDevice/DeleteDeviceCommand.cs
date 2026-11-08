
using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Commands.DeleteDevice;

public sealed record DeleteDeviceCommand(Guid Id) : IRequest<bool>;
