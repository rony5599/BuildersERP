using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantProjectFromRfqAndVendorQuotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "VendorQuotations");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Rfqs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "VendorQuotations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Rfqs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_VendorQuotations_ProjectId",
                table: "VendorQuotations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Rfqs_ProjectId",
                table: "Rfqs",
                column: "ProjectId");

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
    }
}
