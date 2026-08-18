using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyUnitToWbsTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PropertyUnitId",
                table: "WbsTasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WbsTasks_PropertyUnitId",
                table: "WbsTasks",
                column: "PropertyUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_WbsTasks_PropertyUnits_PropertyUnitId",
                table: "WbsTasks",
                column: "PropertyUnitId",
                principalTable: "PropertyUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WbsTasks_PropertyUnits_PropertyUnitId",
                table: "WbsTasks");

            migrationBuilder.DropIndex(
                name: "IX_WbsTasks_PropertyUnitId",
                table: "WbsTasks");

            migrationBuilder.DropColumn(
                name: "PropertyUnitId",
                table: "WbsTasks");
        }
    }
}
