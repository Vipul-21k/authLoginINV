using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace authLogin.Migrations
{
    /// <inheritdoc />
    public partial class Add2FASecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTPExpiry",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "OTP",
                table: "Users",
                newName: "TwoFactorSecretKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TwoFactorSecretKey",
                table: "Users",
                newName: "OTP");

            migrationBuilder.AddColumn<DateTime>(
                name: "OTPExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }
    }
}
