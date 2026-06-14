using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareProjct.web.Migrations
{
    /// <inheritdoc />
    public partial class LoginUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AgreedToPrivacy",
                table: "Register",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AgreedToTerms",
                table: "Register",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AgreementAcceptedOn",
                table: "Register",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Register",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreedToPrivacy",
                table: "Register");

            migrationBuilder.DropColumn(
                name: "AgreedToTerms",
                table: "Register");

            migrationBuilder.DropColumn(
                name: "AgreementAcceptedOn",
                table: "Register");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Register");
        }
    }
}
