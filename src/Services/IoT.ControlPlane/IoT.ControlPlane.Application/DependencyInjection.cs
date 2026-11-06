using IoT.ControlPlane.Application.Modules.Organizations.Commands.CreateOrganization;
using Microsoft.Extensions.DependencyInjection;

namespace IoT.ControlPlane.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CreateOrganizationHandler>();
        services.AddScoped<GetOrganizationByIdHandler>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}