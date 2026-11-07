using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Infrastructure.Persistence;
using IoT.ControlPlane.Application.Modules.Organizations.Contracts;
using IoT.ControlPlane.Application.Modules.Sites.Abstractions;
using IoT.ControlPlane.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoT.ControlPlane.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) {
        var connectionString = configuration.GetConnectionString(
            "ControlPlane");

        if (string.IsNullOrWhiteSpace(connectionString)) {
            throw new InvalidOperationException(
                "Connection string 'ControlPlane' is not configured.");
        }

        services.AddDbContext<ControlPlaneDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<OrganizationRepository>();


        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IOrganizationExistenceChecker>(
            provider => provider.GetRequiredService<OrganizationRepository>());

        return services;
    }
}