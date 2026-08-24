using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectToProcurement : Migration
    {
        /// <inheritdoc />
        private static readonly Guid DefaultBackfillProjectId = new Guid("2ADE6805-5967-4A5D-9C46-584D5EEA3AD1"); // "Sunset Towers"

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "VendorQuotations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: DefaultBackfillProjectId);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Rfqs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: DefaultBackfillProjectId);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "PurchaseReturns",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: DefaultBackfillProjectId);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "PurchaseRequisitions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: DefaultBackfillProjectId);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "GoodsReceives",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: DefaultBackfillProjectId);

            migrationBuilder.CreateIndex(
                name: "IX_VendorQuotations_ProjectId",
                table: "VendorQuotations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Rfqs_ProjectId",
                table: "Rfqs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturns_ProjectId",
                table: "PurchaseReturns",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitions_ProjectId",
                table: "PurchaseRequisitions",
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
                name: "FK_PurchaseRequisitions_Projects_ProjectId",
                table: "PurchaseRequisitions",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Rfqs_Projects_ProjectId",
                table: "Rfqs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorQuotations_Projects_ProjectId",
                table: "VendorQuotations",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceives_Projects_ProjectId",
                table: "GoodsReceives");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequisitions_Projects_ProjectId",
                table: "PurchaseRequisitions");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseReturns_Projects_ProjectId",
                table: "PurchaseReturns");

            migrationBuilder.DropForeignKey(
                name: "FK_Rfqs_Projects_ProjectId",
                table: "Rfqs");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorQuotations_Projects_ProjectId",
                table: "VendorQuotations");

            migrationBuilder.DropIndex(
                name: "IX_VendorQuotations_ProjectId",
                table: "VendorQuotations");

            migrationBuilder.DropIndex(
                name: "IX_Rfqs_ProjectId",
                table: "Rfqs");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseReturns_ProjectId",
                table: "PurchaseReturns");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequisitions_ProjectId",
                table: "PurchaseRequisitions");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceives_ProjectId",
                table: "GoodsReceives");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "VendorQuotations");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Rfqs");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "PurchaseReturns");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "PurchaseRequisitions");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "GoodsReceives");
        }
    }
}
