using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImproveBoqHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "BoqItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BoqItems");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "BoqItems");

            migrationBuilder.RenameColumn(
                name: "UnitOfMeasure",
                table: "BoqItems",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "BoqItems",
                newName: "UnitRate");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "BoqItems",
                newName: "ItemDescription");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "BoqItems",
                newName: "TotalAmount");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "BoqItems",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "BoqItems",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitRate",
                table: "BoqItems",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<long>(
                name: "BoqId",
                table: "BoqItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WorkGroupId",
                table: "BoqItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "BoqItems",
                type: "decimal(38,8)",
                precision: 38,
                scale: 8,
                nullable: false,
                computedColumnSql: "[Quantity] * [UnitRate]",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateTable(
                name: "BoqHeaders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<long>(type: "bigint", nullable: false),
                    BoqName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoqHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoqHeaders_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentGroupId = table.Column<long>(type: "bigint", nullable: true),
                    GroupCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkGroups_WorkGroups_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalTable: "WorkGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("""
                INSERT INTO WorkGroups (ParentGroupId, GroupCode, GroupName, Guid, CreatedAt, IsDeleted) VALUES
                    (NULL, '03', 'Concrete', NEWID(), SYSUTCDATETIME(), 0),
                    (NULL, '04', 'Masonry', NEWID(), SYSUTCDATETIME(), 0),
                    (NULL, '05', 'Metals', NEWID(), SYSUTCDATETIME(), 0),
                    (NULL, '09', 'Finishes', NEWID(), SYSUTCDATETIME(), 0),
                    (NULL, '26', 'Electrical', NEWID(), SYSUTCDATETIME(), 0);

                INSERT INTO WorkGroups (ParentGroupId, GroupCode, GroupName, Guid, CreatedAt, IsDeleted) VALUES
                    ((SELECT Id FROM WorkGroups WHERE GroupCode='03'), '03.30', 'Cast-in-place Concrete', NEWID(), SYSUTCDATETIME(), 0),
                    ((SELECT Id FROM WorkGroups WHERE GroupCode='04'), '04.20', 'Unit Masonry', NEWID(), SYSUTCDATETIME(), 0),
                    ((SELECT Id FROM WorkGroups WHERE GroupCode='05'), '05.20', 'Metal Joists', NEWID(), SYSUTCDATETIME(), 0),
                    ((SELECT Id FROM WorkGroups WHERE GroupCode='09'), '09.30', 'Tiling', NEWID(), SYSUTCDATETIME(), 0),
                    ((SELECT Id FROM WorkGroups WHERE GroupCode='09'), '09.90', 'Painting', NEWID(), SYSUTCDATETIME(), 0),
                    ((SELECT Id FROM WorkGroups WHERE GroupCode='26'), '26.10', 'Electrical Installation', NEWID(), SYSUTCDATETIME(), 0);

                INSERT INTO WorkGroups (ParentGroupId, GroupCode, GroupName, Guid, CreatedAt, IsDeleted) VALUES
                    ((SELECT Id FROM WorkGroups WHERE GroupCode='03.30'), '03.30.01', 'Foundation Concrete', NEWID(), SYSUTCDATETIME(), 0),
                    ((SELECT Id FROM WorkGroups WHERE GroupCode='03.30'), '03.30.02', 'Column & Beam Concrete', NEWID(), SYSUTCDATETIME(), 0),
                    ((SELECT Id FROM WorkGroups WHERE GroupCode='04.20'), '04.20.01', 'Brick Work', NEWID(), SYSUTCDATETIME(), 0),
                    ((SELECT Id FROM WorkGroups WHERE GroupCode='05.20'), '05.20.01', 'Reinforcement', NEWID(), SYSUTCDATETIME(), 0);

                -- BOQ is a new module. Existing placeholder rows are intentionally not migrated.
                DELETE FROM BoqItems;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_BoqItems_Projects_ProjectId",
                table: "BoqItems");

            migrationBuilder.DropIndex(
                name: "IX_BoqItems_ProjectId",
                table: "BoqItems");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "BoqItems");

            migrationBuilder.CreateIndex(
                name: "IX_BoqItems_BoqId",
                table: "BoqItems",
                column: "BoqId");

            migrationBuilder.CreateIndex(
                name: "IX_BoqItems_WorkGroupId",
                table: "BoqItems",
                column: "WorkGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_BoqHeaders_Guid",
                table: "BoqHeaders",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoqHeaders_ProjectId",
                table: "BoqHeaders",
                column: "ProjectId",
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_WorkGroups_GroupCode",
                table: "WorkGroups",
                column: "GroupCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkGroups_Guid",
                table: "WorkGroups",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkGroups_ParentGroupId",
                table: "WorkGroups",
                column: "ParentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoqItems_BoqHeaders_BoqId",
                table: "BoqItems",
                column: "BoqId",
                principalTable: "BoqHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BoqItems_WorkGroups_WorkGroupId",
                table: "BoqItems",
                column: "WorkGroupId",
                principalTable: "WorkGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoqItems_BoqHeaders_BoqId",
                table: "BoqItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BoqItems_WorkGroups_WorkGroupId",
                table: "BoqItems");

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "BoqItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE i SET i.ProjectId = h.ProjectId
                FROM BoqItems i
                INNER JOIN BoqHeaders h ON h.Id = i.BoqId;
                """);

            migrationBuilder.AlterColumn<long>(
                name: "ProjectId",
                table: "BoqItems",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoqItems_ProjectId",
                table: "BoqItems",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoqItems_Projects_ProjectId",
                table: "BoqItems",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropTable(
                name: "BoqHeaders");

            migrationBuilder.DropTable(
                name: "WorkGroups");

            migrationBuilder.DropIndex(
                name: "IX_BoqItems_BoqId",
                table: "BoqItems");

            migrationBuilder.DropColumn(
                name: "BoqId",
                table: "BoqItems");

            migrationBuilder.RenameColumn(
                name: "UnitRate",
                table: "BoqItems",
                newName: "Rate");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "BoqItems",
                newName: "UnitOfMeasure");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "BoqItems",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "ItemDescription",
                table: "BoqItems",
                newName: "Description");

            migrationBuilder.DropIndex(
                name: "IX_BoqItems_WorkGroupId",
                table: "BoqItems");

            migrationBuilder.DropColumn(
                name: "WorkGroupId",
                table: "BoqItems");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "BoqItems",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "BoqItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "UnitOfMeasure",
                table: "BoqItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "BoqItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,8)",
                oldPrecision: 38,
                oldScale: 8,
                oldComputedColumnSql: "[Quantity] * [UnitRate]");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "BoqItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BoqItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "BoqItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

        }
    }
}
