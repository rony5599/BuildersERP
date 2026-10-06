using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseOrderRejectionDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "PurchaseOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "PurchaseOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "PurchaseOrders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "PurchaseOrders");
        }
    }
}
