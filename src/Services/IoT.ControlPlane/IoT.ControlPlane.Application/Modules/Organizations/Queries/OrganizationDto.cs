namespace IoT.ControlPlane.Application.Modules.Organizations.Queries;

public sealed record OrganizationDto(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAtUtc);