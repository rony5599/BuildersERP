using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPoBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoBills",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SupplierInvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoBills_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PoBillDetails",
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
                    PoBillId = table.Column<long>(type: "bigint", nullable: false),
                    PurchaseOrderDetailId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_PoBillDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoBillDetails_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PoBillDetails_PoBills_PoBillId",
                        column: x => x.PoBillId,
                        principalTable: "PoBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PoBillDetails_PurchaseOrderDetails_PurchaseOrderDetailId",
                        column: x => x.PurchaseOrderDetailId,
                        principalTable: "PurchaseOrderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoBillDetails_Guid",
                table: "PoBillDetails",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PoBillDetails_MaterialId",
                table: "PoBillDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_PoBillDetails_PoBillId",
                table: "PoBillDetails",
                column: "PoBillId");

            migrationBuilder.CreateIndex(
                name: "IX_PoBillDetails_PurchaseOrderDetailId",
                table: "PoBillDetails",
                column: "PurchaseOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PoBills_BillNumber",
                table: "PoBills",
                column: "BillNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PoBills_Guid",
                table: "PoBills",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PoBills_PurchaseOrderId",
                table: "PoBills",
                column: "PurchaseOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoBillDetails");

            migrationBuilder.DropTable(
                name: "PoBills");
        }
    }
}
