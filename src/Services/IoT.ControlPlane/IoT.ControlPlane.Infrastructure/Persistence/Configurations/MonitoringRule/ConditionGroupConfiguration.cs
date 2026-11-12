
using IoT.ControlPlane.Domain.Modules.MonitoringRules;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IoT.ControlPlane.Infrastructure.Persistence.Configurations.MonitoringRules;

public sealed class ConditionGroupConfiguration
    : IEntityTypeConfiguration<ConditionGroup>
{
    public void Configure(EntityTypeBuilder<ConditionGroup> builder)
    {
        builder.Property(x => x.LogicalOperator)
            .HasColumnName("logical_operator")
            .HasConversion<int>()
            .IsRequired();

        builder.HasMany(x => x.Nodes)
            .WithOne()
            .HasForeignKey("parent_group_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Nodes)
            .HasField("_nodes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
