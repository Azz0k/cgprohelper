using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateKeeper.Core.Migrations
{
    /// <inheritdoc />
    public partial class BlockedEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockedEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SenderEmail = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE"),
                    RecipientEmail = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE"),
                    Date = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE"),
                    Time = table.Column<string>(type: "TEXT", nullable: false, collation: "NOCASE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedEmails", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedEmails");
        }
    }
}
