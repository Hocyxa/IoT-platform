
using IoT.ControlPlane.Application.Modules.Organizations.Commands.CreateOrganization;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.DeleteOrganization;
using IoT.ControlPlane.Application.Modules.Organizations.Commands.RenameOrganization;
using IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizationById;
using IoT.ControlPlane.Application.Modules.Organizations.Queries.GetOrganizations;
using IoT.ControlPlane.Application.Modules.Sites;

using Microsoft.Extensions.DependencyInjection;

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

        //Services
        services.AddScoped<SiteService>();


        //Utils
        services.AddSingleton(TimeProvider.System);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}
