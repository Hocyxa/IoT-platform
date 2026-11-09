using System.ComponentModel.DataAnnotations;

namespace IoT.ControlPlane.Api.Contracts.Sensors;

public sealed record CreateSensorRequest(
    Guid DeviceId,

    [Required]
    [StringLength(200, MinimumLength = 1)]
    string Name,

    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Code,

    MeasurementTypeRequest MeasurementType,

    [Required]
    [StringLength(30, MinimumLength = 1)]
    string Unit);
