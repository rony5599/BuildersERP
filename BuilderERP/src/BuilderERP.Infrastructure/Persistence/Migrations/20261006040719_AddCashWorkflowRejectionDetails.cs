using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCashWorkflowRejectionDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "CashRequisitions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "CashRequisitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "CashRequisitions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "CashPurchaseOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "CashPurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "CashPurchaseOrders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "CashPoBills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "CashPoBills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "CashPoBills",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "CashRequisitions");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "CashRequisitions");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "CashRequisitions");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "CashPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "CashPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "CashPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "CashPoBills");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "CashPoBills");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "CashPoBills");
        }
    }
}
