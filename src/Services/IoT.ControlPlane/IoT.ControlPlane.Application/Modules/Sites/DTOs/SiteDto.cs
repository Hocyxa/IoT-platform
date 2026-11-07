
namespace IoT.ControlPlane.Application.Modules.Sites.DTOs;

public sealed record SiteDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string? Address,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
