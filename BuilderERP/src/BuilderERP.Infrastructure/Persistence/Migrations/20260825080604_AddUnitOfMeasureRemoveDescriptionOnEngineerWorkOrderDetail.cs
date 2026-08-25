using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitOfMeasureRemoveDescriptionOnEngineerWorkOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "EngineerWorkOrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "UnitOfMeasure",
                table: "EngineerWorkOrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "EngineerWorkOrderDetails");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EngineerWorkOrderDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
