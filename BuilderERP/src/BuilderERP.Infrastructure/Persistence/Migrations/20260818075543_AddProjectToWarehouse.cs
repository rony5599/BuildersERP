using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectToWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: true);

            // Backfill existing warehouses with the earliest project so the column can become required.
            migrationBuilder.Sql(@"
                UPDATE Warehouses
                SET ProjectId = (SELECT TOP 1 Id FROM Projects ORDER BY CreatedAt)
                WHERE ProjectId IS NULL AND EXISTS (SELECT 1 FROM Projects);
            ");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_ProjectId",
                table: "Warehouses",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Projects_ProjectId",
                table: "Warehouses",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Projects_ProjectId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_ProjectId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Warehouses");
        }
    }
}
