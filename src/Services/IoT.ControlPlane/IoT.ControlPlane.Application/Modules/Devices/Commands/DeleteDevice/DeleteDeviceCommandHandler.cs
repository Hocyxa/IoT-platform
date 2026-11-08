
using IoT.ControlPlane.Application.Modules.Devices.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Commands.DeleteDevice;

public sealed class DeleteDeviceCommandHandler(IDeviceRepository deviceRepository) : IRequestHandler<DeleteDeviceCommand, bool>
{
    public async Task<bool> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (device is null)
        {
            return false;
        }
        await deviceRepository.DeleteAsync(device, cancellationToken);

        return true;

    }
}
