using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Infrastructure.Persistence;
using IoT.ControlPlane.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoT.ControlPlane.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(
            "ControlPlane");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'ControlPlane' is not configured.");
        }

        services.AddDbContext<ControlPlaneDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IOrganizationRepository, OrganizationRepository>();

        return services;
    }
}