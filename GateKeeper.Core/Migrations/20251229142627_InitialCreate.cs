using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateKeeper.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllowedDomains",
                columns: table => new
                {
                    Domain = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedDomains", x => x.Domain);
                });

            migrationBuilder.CreateTable(
                name: "ForeingAddresses",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    ReceivedDate = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeingAddresses", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "localMonitoredAddresses",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    IsReplyAllowed = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_localMonitoredAddresses", x => x.Email);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowedDomains");

            migrationBuilder.DropTable(
                name: "ForeingAddresses");

            migrationBuilder.DropTable(
                name: "localMonitoredAddresses");
        }
    }
}
