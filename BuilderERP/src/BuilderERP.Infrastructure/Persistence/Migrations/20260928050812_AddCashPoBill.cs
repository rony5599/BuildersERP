using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCashPoBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashPoBills",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MemoNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CashPurchaseOrderId = table.Column<long>(type: "bigint", nullable: false),
                    RequesterEmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashPoBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashPoBills_CashPurchaseOrders_CashPurchaseOrderId",
                        column: x => x.CashPurchaseOrderId,
                        principalTable: "CashPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashPoBills_Employees_RequesterEmployeeId",
                        column: x => x.RequesterEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CashPoBillDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BilledQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CashPoBillId = table.Column<long>(type: "bigint", nullable: false),
                    CashPurchaseOrderDetailId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_CashPoBillDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashPoBillDetails_CashPoBills_CashPoBillId",
                        column: x => x.CashPoBillId,
                        principalTable: "CashPoBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashPoBillDetails_CashPurchaseOrderDetails_CashPurchaseOrderDetailId",
                        column: x => x.CashPurchaseOrderDetailId,
                        principalTable: "CashPurchaseOrderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashPoBillDetails_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashPoBillDetails_CashPoBillId",
                table: "CashPoBillDetails",
                column: "CashPoBillId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPoBillDetails_CashPurchaseOrderDetailId",
                table: "CashPoBillDetails",
                column: "CashPurchaseOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPoBillDetails_Guid",
                table: "CashPoBillDetails",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashPoBillDetails_MaterialId",
                table: "CashPoBillDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPoBills_BillNumber",
                table: "CashPoBills",
                column: "BillNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CashPoBills_CashPurchaseOrderId",
                table: "CashPoBills",
                column: "CashPurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPoBills_Guid",
                table: "CashPoBills",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashPoBills_RequesterEmployeeId",
                table: "CashPoBills",
                column: "RequesterEmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashPoBillDetails");

            migrationBuilder.DropTable(
                name: "CashPoBills");
        }
    }
}
