namespace IoT.ControlPlane.Application.Modules.Organizations.Contracts;

public interface IOrganizationExistenceChecker
{
    Task<bool> ExistsAsync(
        Guid organizationId,
        CancellationToken cancellationToken);
}