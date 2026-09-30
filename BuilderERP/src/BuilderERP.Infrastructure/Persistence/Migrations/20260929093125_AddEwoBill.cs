using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEwoBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PoBillId",
                table: "SupplierPayments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "EwoBillId",
                table: "SupplierPayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EngineerWorkOrderPaymentHeads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeadName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    EngineerWorkOrderId = table.Column<long>(type: "bigint", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineerWorkOrderPaymentHeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineerWorkOrderPaymentHeads_EngineerWorkOrders_EngineerWorkOrderId",
                        column: x => x.EngineerWorkOrderId,
                        principalTable: "EngineerWorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EwoBills",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContractorBillNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MrrNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MeasuredAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CumulativePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CumulativeDue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PreviouslyCertified = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CertifiedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AdditionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetPayable = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EngineerWorkOrderId = table.Column<long>(type: "bigint", nullable: false),
                    RootWorkOrderId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EwoBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EwoBills_EngineerWorkOrders_EngineerWorkOrderId",
                        column: x => x.EngineerWorkOrderId,
                        principalTable: "EngineerWorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EwoBills_EngineerWorkOrders_RootWorkOrderId",
                        column: x => x.RootWorkOrderId,
                        principalTable: "EngineerWorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EwoBills_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EwoBillAdjustments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EwoBillId = table.Column<long>(type: "bigint", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EwoBillAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EwoBillAdjustments_EwoBills_EwoBillId",
                        column: x => x.EwoBillId,
                        principalTable: "EwoBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EwoBillDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeasuredQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EwoBillId = table.Column<long>(type: "bigint", nullable: false),
                    EngineerWorkOrderDetailId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_EwoBillDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EwoBillDetails_EngineerWorkOrderDetails_EngineerWorkOrderDetailId",
                        column: x => x.EngineerWorkOrderDetailId,
                        principalTable: "EngineerWorkOrderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EwoBillDetails_EwoBills_EwoBillId",
                        column: x => x.EwoBillId,
                        principalTable: "EwoBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EwoBillDetails_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EwoBillHeads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeadName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HeadPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ClaimPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    EwoBillId = table.Column<long>(type: "bigint", nullable: false),
                    EngineerWorkOrderPaymentHeadId = table.Column<long>(type: "bigint", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EwoBillHeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EwoBillHeads_EngineerWorkOrderPaymentHeads_EngineerWorkOrderPaymentHeadId",
                        column: x => x.EngineerWorkOrderPaymentHeadId,
                        principalTable: "EngineerWorkOrderPaymentHeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EwoBillHeads_EwoBills_EwoBillId",
                        column: x => x.EwoBillId,
                        principalTable: "EwoBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_EwoBillId",
                table: "SupplierPayments",
                column: "EwoBillId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SupplierPayments_OneBill",
                table: "SupplierPayments",
                sql: "([PoBillId] IS NOT NULL AND [EwoBillId] IS NULL) OR ([PoBillId] IS NULL AND [EwoBillId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrderPaymentHeads_EngineerWorkOrderId",
                table: "EngineerWorkOrderPaymentHeads",
                column: "EngineerWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineerWorkOrderPaymentHeads_Guid",
                table: "EngineerWorkOrderPaymentHeads",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EwoBillAdjustments_EwoBillId",
                table: "EwoBillAdjustments",
                column: "EwoBillId");

            migrationBuilder.CreateIndex(
                name: "IX_EwoBillAdjustments_Guid",
                table: "EwoBillAdjustments",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EwoBillDetails_EngineerWorkOrderDetailId",
                table: "EwoBillDetails",
                column: "EngineerWorkOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_EwoBillDetails_EwoBillId",
                table: "EwoBillDetails",
                column: "EwoBillId");

            migrationBuilder.CreateIndex(
                name: "IX_EwoBillDetails_Guid",
                table: "EwoBillDetails",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EwoBillDetails_MaterialId",
                table: "EwoBillDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_EwoBillHeads_EngineerWorkOrderPaymentHeadId",
                table: "EwoBillHeads",
                column: "EngineerWorkOrderPaymentHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_EwoBillHeads_EwoBillId",
                table: "EwoBillHeads",
                column: "EwoBillId");

            migrationBuilder.CreateIndex(
                name: "IX_EwoBillHeads_Guid",
                table: "EwoBillHeads",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EwoBills_BillNumber",
                table: "EwoBills",
                column: "BillNumber");

            migrationBuilder.CreateIndex(
                name: "IX_EwoBills_EngineerWorkOrderId",
                table: "EwoBills",
                column: "EngineerWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_EwoBills_Guid",
                table: "EwoBills",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EwoBills_RootWorkOrderId",
                table: "EwoBills",
                column: "RootWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_EwoBills_SupplierId",
                table: "EwoBills",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierPayments_EwoBills_EwoBillId",
                table: "SupplierPayments",
                column: "EwoBillId",
                principalTable: "EwoBills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierPayments_EwoBills_EwoBillId",
                table: "SupplierPayments");

            migrationBuilder.DropTable(
                name: "EwoBillAdjustments");

            migrationBuilder.DropTable(
                name: "EwoBillDetails");

            migrationBuilder.DropTable(
                name: "EwoBillHeads");

            migrationBuilder.DropTable(
                name: "EngineerWorkOrderPaymentHeads");

            migrationBuilder.DropTable(
                name: "EwoBills");

            migrationBuilder.DropIndex(
                name: "IX_SupplierPayments_EwoBillId",
                table: "SupplierPayments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SupplierPayments_OneBill",
                table: "SupplierPayments");

            migrationBuilder.DropColumn(
                name: "EwoBillId",
                table: "SupplierPayments");

            migrationBuilder.AlterColumn<long>(
                name: "PoBillId",
                table: "SupplierPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
