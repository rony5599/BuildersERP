using BuilderERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261008090000_FinalizeBoqMasterDetail")]
public partial class FinalizeBoqMasterDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "ContingencyPercent",
            table: "BoqHeaders",
            type: "decimal(5,2)",
            precision: 5,
            scale: 2,
            nullable: false,
            defaultValue: 2m);

        migrationBuilder.DropIndex(name: "IX_BoqHeaders_ProjectId", table: "BoqHeaders");
        migrationBuilder.CreateIndex(
            name: "IX_BoqHeaders_ProjectId_BoqName_VersionNumber",
            table: "BoqHeaders",
            columns: new[] { "ProjectId", "BoqName", "VersionNumber" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_BoqHeaders_ProjectId_BoqName_VersionNumber", table: "BoqHeaders");
        migrationBuilder.DropColumn(name: "ContingencyPercent", table: "BoqHeaders");
        migrationBuilder.CreateIndex(
            name: "IX_BoqHeaders_ProjectId",
            table: "BoqHeaders",
            column: "ProjectId",
            filter: "[IsActive] = 1");
    }
}
