using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateKeeper.Core.Migrations
{
    /// <inheritdoc />
    public partial class NoCaseCollation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {



            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "localMonitoredAddresses",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ReceivedDate",
                table: "ForeingAddresses",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ForeingAddresses",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AllowedEmails",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Domain",
                table: "AllowedDomains",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllowedEmails",
                table: "AllowedEmails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AllowedEmails_Email",
                table: "AllowedEmails",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AllowedEmails",
                table: "AllowedEmails");

            migrationBuilder.DropIndex(
                name: "IX_AllowedEmails_Email",
                table: "AllowedEmails");

            migrationBuilder.RenameTable(
                name: "AllowedEmails",
                newName: "allowedEmails");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "localMonitoredAddresses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "ReceivedDate",
                table: "ForeingAddresses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ForeingAddresses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "allowedEmails",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldCollation: "NOCASE");

            migrationBuilder.AlterColumn<string>(
                name: "Domain",
                table: "AllowedDomains",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldCollation: "NOCASE");

            migrationBuilder.AddPrimaryKey(
                name: "PK_allowedEmails",
                table: "allowedEmails",
                column: "Id");
        }
    }
}
