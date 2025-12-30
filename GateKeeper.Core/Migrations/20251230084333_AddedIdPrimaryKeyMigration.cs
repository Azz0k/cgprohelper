using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateKeeper.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddedIdPrimaryKeyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_localMonitoredAddresses",
                table: "localMonitoredAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ForeingAddresses",
                table: "ForeingAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllowedDomains",
                table: "AllowedDomains");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "localMonitoredAddresses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ForeingAddresses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AllowedDomains",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_localMonitoredAddresses",
                table: "localMonitoredAddresses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForeingAddresses",
                table: "ForeingAddresses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllowedDomains",
                table: "AllowedDomains",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_localMonitoredAddresses_Email",
                table: "localMonitoredAddresses",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForeingAddresses_Email",
                table: "ForeingAddresses",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AllowedDomains_Domain",
                table: "AllowedDomains",
                column: "Domain",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_localMonitoredAddresses",
                table: "localMonitoredAddresses");

            migrationBuilder.DropIndex(
                name: "IX_localMonitoredAddresses_Email",
                table: "localMonitoredAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ForeingAddresses",
                table: "ForeingAddresses");

            migrationBuilder.DropIndex(
                name: "IX_ForeingAddresses_Email",
                table: "ForeingAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllowedDomains",
                table: "AllowedDomains");

            migrationBuilder.DropIndex(
                name: "IX_AllowedDomains_Domain",
                table: "AllowedDomains");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "localMonitoredAddresses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ForeingAddresses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AllowedDomains");

            migrationBuilder.AddPrimaryKey(
                name: "PK_localMonitoredAddresses",
                table: "localMonitoredAddresses",
                column: "Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForeingAddresses",
                table: "ForeingAddresses",
                column: "Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllowedDomains",
                table: "AllowedDomains",
                column: "Domain");
        }
    }
}
