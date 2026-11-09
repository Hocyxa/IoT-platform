namespace IoT.ControlPlane.Application.Modules.Sites.Contracts;

public interface IOrganizationExistenceChecker
{
    Task<bool> ExistsAsync(
        Guid organizationId,
        CancellationToken cancellationToken);
}
