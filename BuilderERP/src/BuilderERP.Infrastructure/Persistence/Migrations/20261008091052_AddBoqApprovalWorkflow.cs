using System;
using BuilderERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace BuilderERP.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261008091052_AddBoqApprovalWorkflow")]
public partial class AddBoqApprovalWorkflow : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>("ApprovedAt", "BoqHeaders", "datetime2", nullable: true);
        migrationBuilder.AddColumn<string>("ApprovedBy", "BoqHeaders", "nvarchar(max)", nullable: true);
        migrationBuilder.AddColumn<DateTime>("RejectedAt", "BoqHeaders", "datetime2", nullable: true);
        migrationBuilder.AddColumn<string>("RejectedBy", "BoqHeaders", "nvarchar(max)", nullable: true);
        migrationBuilder.AddColumn<string>("RejectionReason", "BoqHeaders", "nvarchar(1000)", maxLength: 1000, nullable: true);
        migrationBuilder.AddColumn<string>("Status", "BoqHeaders", "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Draft");
        migrationBuilder.CreateTable("BoqActionAssignments", table => new
        {
            Id = table.Column<long>("bigint", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
            UserId = table.Column<Guid>("uniqueidentifier", nullable: false),
            CanDraftEdit = table.Column<bool>("bit", nullable: false),
            CanSubmit = table.Column<bool>("bit", nullable: false),
            CanRequestApproval = table.Column<bool>("bit", nullable: false),
            CanApprove = table.Column<bool>("bit", nullable: false),
            CanReject = table.Column<bool>("bit", nullable: false),
            CanCancel = table.Column<bool>("bit", nullable: false),
            Guid = table.Column<Guid>("uniqueidentifier", nullable: false),
            CreatedAt = table.Column<DateTime>("datetime2", nullable: false),
            CreatedBy = table.Column<string>("nvarchar(max)", nullable: true),
            ModifiedAt = table.Column<DateTime>("datetime2", nullable: true),
            ModifiedBy = table.Column<string>("nvarchar(max)", nullable: true),
            IsDeleted = table.Column<bool>("bit", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_BoqActionAssignments", x => x.Id);
            table.ForeignKey("FK_BoqActionAssignments_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
        });
        migrationBuilder.CreateIndex("IX_BoqActionAssignments_Guid", "BoqActionAssignments", "Guid", unique: true);
        migrationBuilder.CreateIndex("IX_BoqActionAssignments_UserId", "BoqActionAssignments", "UserId", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("BoqActionAssignments");
        migrationBuilder.DropColumn("ApprovedAt", "BoqHeaders");
        migrationBuilder.DropColumn("ApprovedBy", "BoqHeaders");
        migrationBuilder.DropColumn("RejectedAt", "BoqHeaders");
        migrationBuilder.DropColumn("RejectedBy", "BoqHeaders");
        migrationBuilder.DropColumn("RejectionReason", "BoqHeaders");
        migrationBuilder.DropColumn("Status", "BoqHeaders");
    }
}
