using System.ComponentModel.DataAnnotations;

namespace IoT.ControlPlane.Api.Contracts.Devices;

public sealed record UpdateDeviceRequest(
    [Required]
    [StringLength(200, MinimumLength = 1)]
    string Name);
