using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Preventyon.Migrations
{
    /// <inheritdoc />
    public partial class notification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Incident_IncidentId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_IncidentId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IncidentId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Notifications",
                newName: "Message");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Notifications",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "IncidentId",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IncidentId",
                table: "Notifications",
                column: "IncidentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Incident_IncidentId",
                table: "Notifications",
                column: "IncidentId",
                principalTable: "Incident",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
