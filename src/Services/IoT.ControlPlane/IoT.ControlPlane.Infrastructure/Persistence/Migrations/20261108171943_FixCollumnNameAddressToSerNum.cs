using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoT.ControlPlane.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixCollumnNameAddressToSerNum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "address",
                schema: "control",
                table: "devices",
                newName: "serial_number");

            migrationBuilder.RenameIndex(
                name: "IX_devices_address",
                schema: "control",
                table: "devices",
                newName: "IX_devices_serial_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "serial_number",
                schema: "control",
                table: "devices",
                newName: "address");

            migrationBuilder.RenameIndex(
                name: "IX_devices_serial_number",
                schema: "control",
                table: "devices",
                newName: "IX_devices_address");
        }
    }
}
