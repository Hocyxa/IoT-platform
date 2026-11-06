
using Microsoft.Extensions.DependencyInjection;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.CreateOrganization;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.RenameOrganization;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.DeleteOrganization;
using IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizationById;
using IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizations;
namespace IoT.ControlPlane.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        //Commands
        services.AddScoped<CreateOrganizationHandler>();
        services.AddScoped<RenameOrganizationHandler>();
        services.AddScoped<DeleteOrganizationHandler>();

        //Queries
        services.AddScoped<GetOrganizationByIdHandler>();
        services.AddScoped<GetOrganizationsHandler>();

        //Utils
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}