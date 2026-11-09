namespace IoT.ControlPlane.Application.Modules.Devices.Contracts;

public interface ISiteExistenceChecker
{
    Task<bool> ExistsAsync(
        Guid siteId,
        CancellationToken cancellationToken);
}
