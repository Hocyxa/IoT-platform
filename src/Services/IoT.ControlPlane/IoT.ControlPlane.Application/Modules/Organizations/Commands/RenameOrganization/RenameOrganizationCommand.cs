namespace IoT.ControlPlane.Application.Modules.Organizations.Commands.RenameOrganization;

public sealed record RenameOrganizationCommand(
    Guid OrganizationId,
    string Name);