using IoT.ControlPlane.Domain.Modules.Sites;
using IoT.ControlPlane.Domain.Modules.Organizations;
using IoT.ControlPlane.Infrastructure.Persistence.Configurations.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IoT.ControlPlane.Infrastructure.Persistence.Configurations;

public sealed class SiteConfiguration
    : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("sites");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasColumnName("address")
            .HasMaxLength(500);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ConfigureAuditableEntity();
    }
}