using System.ComponentModel.DataAnnotations;

namespace IoT.ControlPlane.Api.Contracts.Sensors;

public sealed record UpdateSensorRequest(
    [Required]
    [StringLength(200, MinimumLength = 1)]
    string Name,

    MeasurementTypeRequest MeasurementType,

    [Required]
    [StringLength(30, MinimumLength = 1)]
    string Unit);
