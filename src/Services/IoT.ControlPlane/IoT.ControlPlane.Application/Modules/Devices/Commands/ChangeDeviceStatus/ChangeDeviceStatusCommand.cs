
using IoT.ControlPlane.Domain.Modules.Devices;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Commands.ChangeDeviceStatus;

public sealed record ChangeDeviceStatusCommand(Guid Id, DeviceStatus Status) : IRequest<bool>;
