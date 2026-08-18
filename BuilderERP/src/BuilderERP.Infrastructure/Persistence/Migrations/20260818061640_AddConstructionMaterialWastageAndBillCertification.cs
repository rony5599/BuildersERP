using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddConstructionMaterialWastageAndBillCertification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConsumedQuantity",
                table: "StockIssues",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WastageQuantity",
                table: "StockIssues",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "WastageReason",
                table: "StockIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateNumber",
                table: "RunningBills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CertificationDate",
                table: "RunningBills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertifiedBy",
                table: "RunningBills",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsumedQuantity",
                table: "StockIssues");

            migrationBuilder.DropColumn(
                name: "WastageQuantity",
                table: "StockIssues");

            migrationBuilder.DropColumn(
                name: "WastageReason",
                table: "StockIssues");

            migrationBuilder.DropColumn(
                name: "CertificateNumber",
                table: "RunningBills");

            migrationBuilder.DropColumn(
                name: "CertificationDate",
                table: "RunningBills");

            migrationBuilder.DropColumn(
                name: "CertifiedBy",
                table: "RunningBills");
        }
    }
}
