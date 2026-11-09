using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;

using MediatR;

namespace IoT.ControlPlane.Application.Modules.Sensors.DeleteSensor;

public sealed class DeleteSensorHandler(ISensorRepository repository) : IRequestHandler<DeleteSensorCommand, bool>
{
    public async Task<bool> Handle(DeleteSensorCommand request, CancellationToken cancellationToken)
    {
        var sensor = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (sensor is null)
        {
            return false;
        }
        await repository.DeleteAsync(sensor, cancellationToken);
        return true;
    }
}
