
using IoT.ControlPlane.Domain.Modules.MonitoringRules;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IoT.ControlPlane.Infrastructure.Persistence.Configurations.MonitoringRules;

public sealed class MonitoringConditionConfiguration
    : IEntityTypeConfiguration<MonitoringCondition>
{
    public void Configure(EntityTypeBuilder<MonitoringCondition> builder)
    {
        builder.Property(x => x.ComparisonOperator)
            .HasColumnName("comparison_operator")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Threshold)
            .HasColumnName("threshold")
            .HasPrecision(18, 6)
            .IsRequired();
    }
}
