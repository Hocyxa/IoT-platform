namespace IoT.ControlPlane.Api.Contracts.Sites;

public sealed record SiteResponse(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string? Address,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);