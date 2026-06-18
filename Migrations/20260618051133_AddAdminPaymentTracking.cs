using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareProjct.web.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminPaymentTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerPaidToAdminOn",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPaymentStatus",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NursePaidOn",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NursePayableAmount",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "NursePaymentStatus",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PlatformFee",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPaidToAdminOn",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "CustomerPaymentStatus",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "NursePaidOn",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "NursePayableAmount",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "NursePaymentStatus",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "PlatformFee",
                table: "OrderConfirm");
        }
    }
}
