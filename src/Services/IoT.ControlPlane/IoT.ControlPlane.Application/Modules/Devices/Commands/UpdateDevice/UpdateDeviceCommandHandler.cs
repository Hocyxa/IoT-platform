
using IoT.ControlPlane.Application.Modules.Devices.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Commands.UpdateDevice;

public sealed class UpdateDeviceCommandHandler(IDeviceRepository deviceRepository, TimeProvider timeProvider) : IRequestHandler<UpdateDeviceCommand, bool>
{
    public async Task<bool> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (device is null)
        {
            return false;
        }

        device.UpdateDetails(request.Name, timeProvider.GetUtcNow());

        await deviceRepository.UpdateAsync(device, cancellationToken);
        return true;


    }
}
