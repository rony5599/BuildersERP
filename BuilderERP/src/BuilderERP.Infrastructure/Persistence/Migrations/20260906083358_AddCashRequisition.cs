using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCashRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashRequisitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequiredByDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RequesterEmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    ProjectId = table.Column<long>(type: "bigint", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashRequisitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashRequisitions_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashRequisitions_Employees_RequesterEmployeeId",
                        column: x => x.RequesterEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashRequisitions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CashRequisitionDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "int", nullable: false),
                    EstimatedUnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CashRequisitionId = table.Column<long>(type: "bigint", nullable: false),
                    MaterialId = table.Column<long>(type: "bigint", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashRequisitionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashRequisitionDetails_CashRequisitions_CashRequisitionId",
                        column: x => x.CashRequisitionId,
                        principalTable: "CashRequisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashRequisitionDetails_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashRequisitionDetails_CashRequisitionId",
                table: "CashRequisitionDetails",
                column: "CashRequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CashRequisitionDetails_Guid",
                table: "CashRequisitionDetails",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashRequisitionDetails_MaterialId",
                table: "CashRequisitionDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_CashRequisitions_DepartmentId",
                table: "CashRequisitions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CashRequisitions_Guid",
                table: "CashRequisitions",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashRequisitions_ProjectId",
                table: "CashRequisitions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CashRequisitions_RequesterEmployeeId",
                table: "CashRequisitions",
                column: "RequesterEmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashRequisitionDetails");

            migrationBuilder.DropTable(
                name: "CashRequisitions");
        }
    }
}
