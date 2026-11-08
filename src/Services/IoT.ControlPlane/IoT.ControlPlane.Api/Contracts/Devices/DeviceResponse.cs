namespace IoT.ControlPlane.Api.Contracts.Devices;

public sealed record DeviceResponse(
    Guid Id,
    Guid SiteId,
    string Name,
    string SerialNumber,
    string Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
