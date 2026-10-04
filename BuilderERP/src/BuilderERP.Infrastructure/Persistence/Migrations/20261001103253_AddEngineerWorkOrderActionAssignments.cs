using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEngineerWorkOrderActionAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EngineerWorkOrderActionAssignments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanDraftEdit = table.Column<bool>(type: "bit", nullable: false),
                    CanSubmit = table.Column<bool>(type: "bit", nullable: false),
                    CanRequestApproval = table.Column<bool>(type: "bit", nullable: false),
                    CanApprove = table.Column<bool>(type: "bit", nullable: false),
                    CanReject = table.Column<bool>(type: "bit", nullable: false),
                    CanCancel = table.Column<bool>(type: "bit", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineerWorkOrderActionAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrderActionAssignments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrderActionAssignments_Guid",
                table: "EngineerWorkOrderActionAssignments",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrderActionAssignments_UserId",
                table: "EngineerWorkOrderActionAssignments",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EngineerWorkOrderActionAssignments");
        }
    }
}
