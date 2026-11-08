using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoT.ControlPlane.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sites_organizations_OrganizationId",
                schema: "control",
                table: "sites");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                schema: "control",
                table: "sites",
                newName: "organization_id");

            migrationBuilder.RenameIndex(
                name: "IX_sites_OrganizationId",
                schema: "control",
                table: "sites",
                newName: "IX_sites_organization_id");

            migrationBuilder.CreateTable(
                name: "devices",
                schema: "control",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.id);
                    table.ForeignKey(
                        name: "FK_devices_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "control",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_devices_address",
                schema: "control",
                table: "devices",
                column: "address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_devices_site_id",
                schema: "control",
                table: "devices",
                column: "site_id");

            migrationBuilder.AddForeignKey(
                name: "FK_sites_organizations_organization_id",
                schema: "control",
                table: "sites",
                column: "organization_id",
                principalSchema: "control",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sites_organizations_organization_id",
                schema: "control",
                table: "sites");

            migrationBuilder.DropTable(
                name: "devices",
                schema: "control");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                schema: "control",
                table: "sites",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_sites_organization_id",
                schema: "control",
                table: "sites",
                newName: "IX_sites_OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_sites_organizations_OrganizationId",
                schema: "control",
                table: "sites",
                column: "OrganizationId",
                principalSchema: "control",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
