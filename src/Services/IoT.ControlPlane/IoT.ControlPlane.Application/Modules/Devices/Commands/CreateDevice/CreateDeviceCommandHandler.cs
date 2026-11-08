using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Sites.Contracts;
using IoT.ControlPlane.Domain.Modules.Devices;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Devices.Commands.CreateDevice;

public sealed class CreateDeviceCommandHandler(
    IDeviceRepository deviceRepository,
    ISiteExistenceChecker siteExistenceChecker,
    TimeProvider timeProvider
) : IRequestHandler<CreateDeviceCommand, Guid>
{
    public async Task<Guid> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
    {
        var siteExists = await siteExistenceChecker.ExistsAsync(
            request.SiteId,
            cancellationToken);

        if (!siteExists)
        {
            throw new KeyNotFoundException(
                $"Site with ID '{request.SiteId}' was not found.");
        }

        var device = Device.Create(
            Guid.NewGuid(),
            request.SiteId,
            request.Name,
            request.SerialNumber,
            timeProvider.GetUtcNow());

        await deviceRepository.AddAsync(
            device,
            cancellationToken);

        return device.Id;
    }
}
