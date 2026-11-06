using IoT.ControlPlane.Domain.Modules.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IoT.ControlPlane.Infrastructure.Persistence;

public sealed class ControlPlaneDbContext(
    DbContextOptions<ControlPlaneDbContext> options)
    : DbContext(options)
{
    public DbSet<Organization> Organizations => Set<Organization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("control");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ControlPlaneDbContext).Assembly);
    }
}