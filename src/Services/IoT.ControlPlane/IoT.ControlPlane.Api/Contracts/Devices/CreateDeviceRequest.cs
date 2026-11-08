using System.ComponentModel.DataAnnotations;

namespace IoT.ControlPlane.Api.Contracts.Devices;

public sealed record CreateDeviceRequest(
    Guid SiteId,

    [Required]
    [StringLength(200, MinimumLength = 1)]
    string Name,

    [Required]
    [StringLength(100, MinimumLength = 1)]
    string SerialNumber);
