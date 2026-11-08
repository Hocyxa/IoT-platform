using IoT.ControlPlane.Application.Modules.Devices.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Commands.ChangeDeviceStatus;

public sealed class ChangeDeviceStatusCommandHandler(IDeviceRepository deviceRepository, TimeProvider timeProvider) : IRequestHandler<ChangeDeviceStatusCommand, bool>
{
    public async Task<bool> Handle(ChangeDeviceStatusCommand request, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (device is null)
        {
            return false;
        }

        device.ChangeStatus(request.Status, timeProvider.GetUtcNow());

        await deviceRepository.UpdateAsync(device, cancellationToken);

        return true;

    }
}
