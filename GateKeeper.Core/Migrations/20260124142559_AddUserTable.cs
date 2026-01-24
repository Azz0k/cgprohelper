using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateKeeper.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE"),
                    Hash = table.Column<string>(type: "TEXT", nullable: false, collation: "BINARY"),
                    FullName = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE"),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    TokenVersion = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: -2147483648),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
