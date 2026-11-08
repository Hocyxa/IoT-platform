namespace IoT.ControlPlane.Application.Modules.Sites.Contracts;

public interface ISiteExistenceChecker
{
    Task<bool> ExistsAsync(
        Guid siteId,
        CancellationToken cancellationToken);
}
