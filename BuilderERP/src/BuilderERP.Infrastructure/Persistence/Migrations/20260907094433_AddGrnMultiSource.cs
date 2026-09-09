using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGrnMultiSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PurchaseOrderId",
                table: "GoodsReceives",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "CashPurchaseOrderId",
                table: "GoodsReceives",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EngineerWorkOrderId",
                table: "GoodsReceives",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceType",
                table: "GoodsReceives",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "PurchaseOrderDetailId",
                table: "GoodsReceiveDetails",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "CashPurchaseOrderDetailId",
                table: "GoodsReceiveDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EngineerWorkOrderDetailId",
                table: "GoodsReceiveDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceivedAmount",
                table: "EngineerWorkOrders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceivedQuantity",
                table: "EngineerWorkOrderDetails",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceives_CashPurchaseOrderId",
                table: "GoodsReceives",
                column: "CashPurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceives_EngineerWorkOrderId",
                table: "GoodsReceives",
                column: "EngineerWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiveDetails_CashPurchaseOrderDetailId",
                table: "GoodsReceiveDetails",
                column: "CashPurchaseOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiveDetails_EngineerWorkOrderDetailId",
                table: "GoodsReceiveDetails",
                column: "EngineerWorkOrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceiveDetails_CashPurchaseOrderDetails_CashPurchaseOrderDetailId",
                table: "GoodsReceiveDetails",
                column: "CashPurchaseOrderDetailId",
                principalTable: "CashPurchaseOrderDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceiveDetails_EngineerWorkOrderDetails_EngineerWorkOrderDetailId",
                table: "GoodsReceiveDetails",
                column: "EngineerWorkOrderDetailId",
                principalTable: "EngineerWorkOrderDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceives_CashPurchaseOrders_CashPurchaseOrderId",
                table: "GoodsReceives",
                column: "CashPurchaseOrderId",
                principalTable: "CashPurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceives_EngineerWorkOrders_EngineerWorkOrderId",
                table: "GoodsReceives",
                column: "EngineerWorkOrderId",
                principalTable: "EngineerWorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceiveDetails_CashPurchaseOrderDetails_CashPurchaseOrderDetailId",
                table: "GoodsReceiveDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceiveDetails_EngineerWorkOrderDetails_EngineerWorkOrderDetailId",
                table: "GoodsReceiveDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceives_CashPurchaseOrders_CashPurchaseOrderId",
                table: "GoodsReceives");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceives_EngineerWorkOrders_EngineerWorkOrderId",
                table: "GoodsReceives");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceives_CashPurchaseOrderId",
                table: "GoodsReceives");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceives_EngineerWorkOrderId",
                table: "GoodsReceives");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceiveDetails_CashPurchaseOrderDetailId",
                table: "GoodsReceiveDetails");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceiveDetails_EngineerWorkOrderDetailId",
                table: "GoodsReceiveDetails");

            migrationBuilder.DropColumn(
                name: "CashPurchaseOrderId",
                table: "GoodsReceives");

            migrationBuilder.DropColumn(
                name: "EngineerWorkOrderId",
                table: "GoodsReceives");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "GoodsReceives");

            migrationBuilder.DropColumn(
                name: "CashPurchaseOrderDetailId",
                table: "GoodsReceiveDetails");

            migrationBuilder.DropColumn(
                name: "EngineerWorkOrderDetailId",
                table: "GoodsReceiveDetails");

            migrationBuilder.DropColumn(
                name: "ReceivedAmount",
                table: "EngineerWorkOrders");

            migrationBuilder.DropColumn(
                name: "ReceivedQuantity",
                table: "EngineerWorkOrderDetails");

            migrationBuilder.AlterColumn<long>(
                name: "PurchaseOrderId",
                table: "GoodsReceives",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PurchaseOrderDetailId",
                table: "GoodsReceiveDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
