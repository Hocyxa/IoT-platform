
using IoT.ControlPlane.Domain.Modules.MonitoringRules;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IoT.ControlPlane.Infrastructure.Persistence.Configurations.MonitoringRules;

public sealed class ConditionNodeConfiguration
    : IEntityTypeConfiguration<ConditionNode>
{
    public void Configure(EntityTypeBuilder<ConditionNode> builder)
    {
        builder.ToTable("monitoring_rule_condition_nodes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.HasDiscriminator<string>("node_type")
            .HasValue<ConditionGroup>("group")
            .HasValue<MonitoringCondition>("condition");
    }
}
