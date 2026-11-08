using IoT.ControlPlane.Domain.Modules.Devices;

namespace IoT.ControlPlane.Application.Modules.Devices.DTOs;

public sealed record DeviceDto(
    Guid Id,
    Guid SiteId,
    string Name,
    string SerialNumber,
    DeviceStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
