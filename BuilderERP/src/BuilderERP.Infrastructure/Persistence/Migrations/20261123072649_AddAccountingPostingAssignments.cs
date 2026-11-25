using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingPostingAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountingPostingAssignments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanPostPoBill = table.Column<bool>(type: "bit", nullable: false),
                    CanPostEwoBill = table.Column<bool>(type: "bit", nullable: false),
                    CanPostSupplierPayment = table.Column<bool>(type: "bit", nullable: false),
                    CanPostCustomerReceipt = table.Column<bool>(type: "bit", nullable: false),
                    CanPostManualJournal = table.Column<bool>(type: "bit", nullable: false),
                    CanReverse = table.Column<bool>(type: "bit", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingPostingAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingPostingAssignments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPostingAssignments_Guid",
                table: "AccountingPostingAssignments",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPostingAssignments_UserId",
                table: "AccountingPostingAssignments",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingPostingAssignments");
        }
    }
}
