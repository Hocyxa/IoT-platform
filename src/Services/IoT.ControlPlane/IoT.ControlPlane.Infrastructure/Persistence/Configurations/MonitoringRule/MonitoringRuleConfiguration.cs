
using IoT.ControlPlane.Domain.Modules.Devices;
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.Sensors;
using IoT.ControlPlane.Infrastructure.Persistence.Configurations.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IoT.ControlPlane.Infrastructure.Persistence.Configurations.MonitoringRules;

public sealed class MonitoringRuleConfiguration
    : IEntityTypeConfiguration<MonitoringRule>
{
    public void Configure(EntityTypeBuilder<MonitoringRule> builder)
    {
        builder.ToTable("monitoring_rules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.DeviceId)
            .HasColumnName("device_id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(x => x.Severity)
            .HasColumnName("severity")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsEnabled)
            .HasColumnName("is_enabled")
            .IsRequired();

        builder.Property(x => x.MeasurementType)
            .HasColumnName("measurement_type")
            .HasConversion<int?>();

        builder.HasOne<Device>()
            .WithMany()
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RootConditionGroup)
            .WithOne()
            .HasForeignKey<ConditionGroup>("monitoring_rule_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.RootConditionGroup)
            .HasField("_rootConditionGroup")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.Sensors, sensorBuilder =>
        {
            sensorBuilder.ToTable("monitoring_rule_sensors");

            sensorBuilder.WithOwner()
                .HasForeignKey("monitoring_rule_id");

            sensorBuilder.HasKey(
                "monitoring_rule_id",
                nameof(MonitoringRuleSensor.SensorId));

            sensorBuilder.Property(x => x.SensorId)
                .HasColumnName("sensor_id")
                .ValueGeneratedNever();

            sensorBuilder.HasOne<Sensor>()
                .WithMany()
                .HasForeignKey(x => x.SensorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Navigation(x => x.Sensors)
            .HasField("_sensors")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.ConfigureAuditableEntity();
    }
}
