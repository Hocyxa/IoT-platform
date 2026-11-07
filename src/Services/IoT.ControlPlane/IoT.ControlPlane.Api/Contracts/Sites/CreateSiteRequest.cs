using System.ComponentModel.DataAnnotations;

namespace IoT.ControlPlane.Api.Contracts.Sites;

public sealed record CreateSiteRequest(
    [Required]
    Guid OrganizationId,

    [Required]
    [StringLength(200, MinimumLength = 1)]
    string Name,

    [StringLength(500)]
    string? Address);