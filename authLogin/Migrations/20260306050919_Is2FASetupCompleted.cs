using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace authLogin.Migrations
{
    /// <inheritdoc />
    public partial class Is2FASetupCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Is2FASetupCompleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is2FASetupCompleted",
                table: "Users");
        }
    }
}
