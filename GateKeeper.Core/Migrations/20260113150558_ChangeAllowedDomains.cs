using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateKeeper.Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAllowedDomains : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isManuallyAdded",
                table: "AllowedDomains",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isManuallyAdded",
                table: "AllowedDomains");
        }
    }
}
