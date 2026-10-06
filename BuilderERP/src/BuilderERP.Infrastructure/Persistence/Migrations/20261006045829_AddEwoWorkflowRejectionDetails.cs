using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEwoWorkflowRejectionDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "EwoBills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "EwoBills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "EwoBills",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "EngineerWorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "EngineerWorkOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "EngineerWorkOrders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "EngineerWorkOrderRequisitions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "EngineerWorkOrderRequisitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "EngineerWorkOrderRequisitions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "EwoBills");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "EwoBills");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "EwoBills");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "EngineerWorkOrders");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "EngineerWorkOrders");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "EngineerWorkOrders");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "EngineerWorkOrderRequisitions");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "EngineerWorkOrderRequisitions");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "EngineerWorkOrderRequisitions");
        }
    }
}
