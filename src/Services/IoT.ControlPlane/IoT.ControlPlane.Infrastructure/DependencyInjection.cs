using IoT.ControlPlane.Application.Modules.Devices.Abstractions;
using IoT.ControlPlane.Application.Modules.Devices.Contracts;
using IoT.ControlPlane.Application.Modules.Organizations.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.Abstractions;
using IoT.ControlPlane.Application.Modules.Sensors.Contracts;
using IoT.ControlPlane.Application.Modules.Sites.Abstractions;
using IoT.ControlPlane.Application.Modules.Sites.Contracts;
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

        services.AddScoped<OrganizationRepository>();
        services.AddScoped<SiteRepository>();
        services.AddScoped<DeviceRepository>();
        services.AddScoped<SensorRepository>();



        services.AddScoped<ISensorRepository>(
            provider => provider.GetRequiredService<SensorRepository>());

        services.AddScoped<IDeviceRepository>(
            provider => provider.GetRequiredService<DeviceRepository>());
        services.AddScoped<ISensorDeviceExistenceChecker>(
            provider => provider.GetRequiredService<DeviceRepository>());

        services.AddScoped<IOrganizationRepository>(
            provider => provider.GetRequiredService<OrganizationRepository>());
        services.AddScoped<IOrganizationExistenceChecker>(
            provider => provider.GetRequiredService<OrganizationRepository>());

        services.AddScoped<ISiteRepository>(
            provider => provider.GetRequiredService<SiteRepository>());
        services.AddScoped<ISiteExistenceChecker>(
            provider => provider.GetRequiredService<SiteRepository>());

        return services;
    }
}
