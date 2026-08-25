using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEngineerWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EngineerWorkOrderRequisitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequisitionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequiredByDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineerWorkOrderRequisitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrderRequisitions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EngineerWorkOrderRequisitionDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "int", nullable: false),
                    EstimatedUnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngineerWorkOrderRequisitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineerWorkOrderRequisitionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrderRequisitionDetails_EngineerWorkOrderRequisitions_EngineerWorkOrderRequisitionId",
                        column: x => x.EngineerWorkOrderRequisitionId,
                        principalTable: "EngineerWorkOrderRequisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrderRequisitionDetails_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EngineerWorkOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkOrderNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EngineerWorkOrderRequisitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MotherWorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RevisionNo = table.Column<int>(type: "int", nullable: false),
                    IsLatestRevision = table.Column<bool>(type: "bit", nullable: false),
                    RevisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TermsAndCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PreviousWorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineerWorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrders_EngineerWorkOrderRequisitions_EngineerWorkOrderRequisitionId",
                        column: x => x.EngineerWorkOrderRequisitionId,
                        principalTable: "EngineerWorkOrderRequisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrders_EngineerWorkOrders_MotherWorkOrderId",
                        column: x => x.MotherWorkOrderId,
                        principalTable: "EngineerWorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrders_EngineerWorkOrders_PreviousWorkOrderId",
                        column: x => x.PreviousWorkOrderId,
                        principalTable: "EngineerWorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EngineerWorkOrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngineerWorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineerWorkOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrderDetails_EngineerWorkOrders_EngineerWorkOrderId",
                        column: x => x.EngineerWorkOrderId,
                        principalTable: "EngineerWorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrderDetails_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrderDetails_EngineerWorkOrderId",
                table: "EngineerWorkOrderDetails",
                column: "EngineerWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrderDetails_MaterialId",
                table: "EngineerWorkOrderDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrderRequisitionDetails_EngineerWorkOrderRequisitionId",
                table: "EngineerWorkOrderRequisitionDetails",
                column: "EngineerWorkOrderRequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrderRequisitionDetails_MaterialId",
                table: "EngineerWorkOrderRequisitionDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrderRequisitions_ProjectId",
                table: "EngineerWorkOrderRequisitions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrders_EngineerWorkOrderRequisitionId",
                table: "EngineerWorkOrders",
                column: "EngineerWorkOrderRequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrders_MotherWorkOrderId",
                table: "EngineerWorkOrders",
                column: "MotherWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrders_PreviousWorkOrderId",
                table: "EngineerWorkOrders",
                column: "PreviousWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrders_SupplierId",
                table: "EngineerWorkOrders",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EngineerWorkOrderDetails");

            migrationBuilder.DropTable(
                name: "EngineerWorkOrderRequisitionDetails");

            migrationBuilder.DropTable(
                name: "EngineerWorkOrders");

            migrationBuilder.DropTable(
                name: "EngineerWorkOrderRequisitions");
        }
    }
}
