using System.ComponentModel.DataAnnotations;

namespace IoT.ControlPlane.Api.Contracts.Sites;

public sealed record UpdateSiteRequest(
    [Required]
    [StringLength(200, MinimumLength = 1)]
    string Name,

    [StringLength(500)]
    string? Address);