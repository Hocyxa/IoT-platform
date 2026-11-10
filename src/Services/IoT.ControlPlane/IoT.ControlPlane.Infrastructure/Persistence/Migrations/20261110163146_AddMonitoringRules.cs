using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoT.ControlPlane.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitoringRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "monitoring_rules",
                schema: "control",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    measurement_type = table.Column<int>(type: "integer", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monitoring_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_monitoring_rules_devices_device_id",
                        column: x => x.device_id,
                        principalSchema: "control",
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "monitoring_rule_condition_nodes",
                schema: "control",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    node_type = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    parent_group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    logical_operator = table.Column<int>(type: "integer", nullable: true),
                    monitoring_rule_id = table.Column<Guid>(type: "uuid", nullable: true),
                    comparison_operator = table.Column<int>(type: "integer", nullable: true),
                    threshold = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monitoring_rule_condition_nodes", x => x.id);
                    table.ForeignKey(
                        name: "FK_monitoring_rule_condition_nodes_monitoring_rule_condition_n~",
                        column: x => x.parent_group_id,
                        principalSchema: "control",
                        principalTable: "monitoring_rule_condition_nodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_monitoring_rule_condition_nodes_monitoring_rules_monitoring~",
                        column: x => x.monitoring_rule_id,
                        principalSchema: "control",
                        principalTable: "monitoring_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "monitoring_rule_sensors",
                schema: "control",
                columns: table => new
                {
                    sensor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    monitoring_rule_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monitoring_rule_sensors", x => new { x.monitoring_rule_id, x.sensor_id });
                    table.ForeignKey(
                        name: "FK_monitoring_rule_sensors_monitoring_rules_monitoring_rule_id",
                        column: x => x.monitoring_rule_id,
                        principalSchema: "control",
                        principalTable: "monitoring_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_monitoring_rule_sensors_sensors_sensor_id",
                        column: x => x.sensor_id,
                        principalSchema: "control",
                        principalTable: "sensors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_monitoring_rule_condition_nodes_monitoring_rule_id",
                schema: "control",
                table: "monitoring_rule_condition_nodes",
                column: "monitoring_rule_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_monitoring_rule_condition_nodes_parent_group_id",
                schema: "control",
                table: "monitoring_rule_condition_nodes",
                column: "parent_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_monitoring_rule_sensors_sensor_id",
                schema: "control",
                table: "monitoring_rule_sensors",
                column: "sensor_id");

            migrationBuilder.CreateIndex(
                name: "IX_monitoring_rules_device_id",
                schema: "control",
                table: "monitoring_rules",
                column: "device_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "monitoring_rule_condition_nodes",
                schema: "control");

            migrationBuilder.DropTable(
                name: "monitoring_rule_sensors",
                schema: "control");

            migrationBuilder.DropTable(
                name: "monitoring_rules",
                schema: "control");
        }
    }
}
