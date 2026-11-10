

namespace IoT.ControlPlane.Domain.Modules.MonitoringRules;

public sealed class MonitoringRuleSensor
{
    public Guid SensorId { get; private set; }

    private MonitoringRuleSensor()
    {
    }

    internal MonitoringRuleSensor(Guid sensorId)
    {
        if (sensorId == Guid.Empty)
        {
            throw new ArgumentException(
                "Sensor ID cannot be empty.",
                nameof(sensorId));
        }

        SensorId = sensorId;
    }
}
