using IoT.ControlPlane.Domain.Modules.Devices;
using IoT.ControlPlane.Domain.Modules.Organizations;
using IoT.ControlPlane.Domain.Modules.Sites;

using Microsoft.EntityFrameworkCore;

namespace IoT.ControlPlane.Infrastructure.Persistence;

public sealed class ControlPlaneDbContext(
    DbContextOptions<ControlPlaneDbContext> options)
    : DbContext(options)
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Device> Devices => Set<Device>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("control");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ControlPlaneDbContext).Assembly);
    }
}
