using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantProjectFromGoodsReceiveAndPurchaseReturn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceives_Projects_ProjectId",
                table: "GoodsReceives");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseReturns_Projects_ProjectId",
                table: "PurchaseReturns");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseReturns_ProjectId",
                table: "PurchaseReturns");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceives_ProjectId",
                table: "GoodsReceives");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "PurchaseReturns");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "GoodsReceives");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "PurchaseReturns",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "GoodsReceives",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturns_ProjectId",
                table: "PurchaseReturns",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceives_ProjectId",
                table: "GoodsReceives",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceives_Projects_ProjectId",
                table: "GoodsReceives",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseReturns_Projects_ProjectId",
                table: "PurchaseReturns",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
