namespace IoT.ControlPlane.Api.Contracts.Organizations;

public sealed record OrganizationResponse(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAtUtc);