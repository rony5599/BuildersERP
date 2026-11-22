using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JournalVouchers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VoucherDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceId = table.Column<long>(type: "bigint", nullable: true),
                    ProjectId = table.Column<long>(type: "bigint", nullable: true),
                    CostCenterId = table.Column<long>(type: "bigint", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReversalVoucherId = table.Column<long>(type: "bigint", nullable: true),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalVouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalVouchers_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalVouchers_JournalVouchers_ReversalVoucherId",
                        column: x => x.ReversalVoucherId,
                        principalTable: "JournalVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalVouchers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LedgerAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    AllowPosting = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LedgerAccounts_LedgerAccounts_ParentId",
                        column: x => x.ParentId,
                        principalTable: "LedgerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JournalVoucherLines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalVoucherId = table.Column<long>(type: "bigint", nullable: false),
                    LedgerAccountId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalVoucherLines", x => x.Id);
                    table.CheckConstraint("CK_JournalVoucherLine_OneSide", "([Debit] > 0 AND [Credit] = 0) OR ([Credit] > 0 AND [Debit] = 0)");
                    table.ForeignKey(
                        name: "FK_JournalVoucherLines_JournalVouchers_JournalVoucherId",
                        column: x => x.JournalVoucherId,
                        principalTable: "JournalVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalVoucherLines_LedgerAccounts_LedgerAccountId",
                        column: x => x.LedgerAccountId,
                        principalTable: "LedgerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalVoucherLines_Guid",
                table: "JournalVoucherLines",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalVoucherLines_JournalVoucherId_LineNumber",
                table: "JournalVoucherLines",
                columns: new[] { "JournalVoucherId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalVoucherLines_LedgerAccountId",
                table: "JournalVoucherLines",
                column: "LedgerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalVouchers_CostCenterId",
                table: "JournalVouchers",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalVouchers_Guid",
                table: "JournalVouchers",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalVouchers_ProjectId",
                table: "JournalVouchers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalVouchers_ReversalVoucherId",
                table: "JournalVouchers",
                column: "ReversalVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalVouchers_SourceType_SourceId",
                table: "JournalVouchers",
                columns: new[] { "SourceType", "SourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalVouchers_VoucherNumber",
                table: "JournalVouchers",
                column: "VoucherNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerAccounts_Code",
                table: "LedgerAccounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerAccounts_Guid",
                table: "LedgerAccounts",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerAccounts_ParentId",
                table: "LedgerAccounts",
                column: "ParentId");

            migrationBuilder.Sql(@"
CREATE PROCEDURE dbo.usp_Accounting_PostJournalVoucher
    @VoucherId bigint,
    @PostedBy nvarchar(256)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Status int;
        SELECT @Status = [Status] FROM dbo.JournalVouchers WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @VoucherId AND IsDeleted = 0;
        IF @Status IS NULL THROW 51000, 'Journal voucher was not found.', 1;
        IF @Status <> 1 THROW 51001, 'Only a draft voucher can be posted.', 1;
        IF (SELECT COUNT(*) FROM dbo.JournalVoucherLines WHERE JournalVoucherId=@VoucherId AND IsDeleted=0) < 2
            THROW 51002, 'A voucher requires at least two lines.', 1;
        IF EXISTS (
            SELECT 1 FROM dbo.JournalVoucherLines l
            LEFT JOIN dbo.LedgerAccounts a ON a.Id=l.LedgerAccountId
            WHERE l.JournalVoucherId=@VoucherId AND l.IsDeleted=0
              AND (a.Id IS NULL OR a.IsDeleted=1 OR a.IsActive=0 OR a.AllowPosting=0))
            THROW 51003, 'Every line must use an active posting account.', 1;
        DECLARE @Debit decimal(19,2), @Credit decimal(19,2);
        SELECT @Debit=SUM(Debit), @Credit=SUM(Credit) FROM dbo.JournalVoucherLines
        WHERE JournalVoucherId=@VoucherId AND IsDeleted=0;
        IF ISNULL(@Debit,0)<=0 OR @Debit<>@Credit
            THROW 51004, 'Voucher debit and credit totals must be equal and greater than zero.', 1;
        UPDATE dbo.JournalVouchers SET [Status]=2, PostedAt=SYSUTCDATETIME(), PostedBy=@PostedBy,
            ModifiedAt=SYSUTCDATETIME(), ModifiedBy=@PostedBy WHERE Id=@VoucherId;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END");

            migrationBuilder.Sql(@"
CREATE PROCEDURE dbo.usp_Accounting_ReverseJournalVoucher
    @VoucherId bigint,
    @ReversalDate date,
    @Reason nvarchar(500),
    @ReversedBy nvarchar(256),
    @ReversalVoucherId bigint OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Status int, @OriginalNumber nvarchar(50), @ProjectId bigint, @CostCenterId bigint;
        SELECT @Status=[Status], @OriginalNumber=VoucherNumber, @ProjectId=ProjectId, @CostCenterId=CostCenterId
        FROM dbo.JournalVouchers WITH (UPDLOCK, HOLDLOCK) WHERE Id=@VoucherId AND IsDeleted=0;
        IF @Status IS NULL THROW 51010, 'Journal voucher was not found.', 1;
        IF @Status<>2 THROW 51011, 'Only a posted voucher can be reversed.', 1;
        IF EXISTS(SELECT 1 FROM dbo.JournalVouchers WHERE ReversalVoucherId=@VoucherId AND IsDeleted=0)
            THROW 51012, 'A reversal voucher cannot be reversed.', 1;

        DECLARE @Prefix nvarchar(20)='JV-'+CONVERT(char(6),@ReversalDate,112)+'-';
        DECLARE @Sequence int=ISNULL((SELECT MAX(TRY_CONVERT(int,SUBSTRING(VoucherNumber,LEN(@Prefix)+1,10)))
            FROM dbo.JournalVouchers WITH (UPDLOCK,HOLDLOCK) WHERE VoucherNumber LIKE @Prefix+'%'),0)+1;
        DECLARE @Number nvarchar(50)=@Prefix+RIGHT('00000'+CONVERT(varchar(10),@Sequence),5);

        INSERT dbo.JournalVouchers(VoucherNumber,VoucherDate,Narration,[Status],SourceType,SourceId,ProjectId,
            CostCenterId,PostedAt,PostedBy,ReversalVoucherId,[Guid],CreatedAt,CreatedBy,IsDeleted)
        VALUES(@Number,@ReversalDate,'Reversal of '+@OriginalNumber+': '+@Reason,2,'Reversal',@VoucherId,@ProjectId,
            @CostCenterId,SYSUTCDATETIME(),@ReversedBy,@VoucherId,NEWID(),SYSUTCDATETIME(),@ReversedBy,0);
        SET @ReversalVoucherId=SCOPE_IDENTITY();

        INSERT dbo.JournalVoucherLines(JournalVoucherId,LedgerAccountId,Description,Debit,Credit,LineNumber,
            [Guid],CreatedAt,CreatedBy,IsDeleted)
        SELECT @ReversalVoucherId,LedgerAccountId,'Reversal: '+ISNULL(Description,''),Credit,Debit,LineNumber,
            NEWID(),SYSUTCDATETIME(),@ReversedBy,0
        FROM dbo.JournalVoucherLines WHERE JournalVoucherId=@VoucherId AND IsDeleted=0;

        UPDATE dbo.JournalVouchers SET [Status]=3, ReversalVoucherId=@ReversalVoucherId,
            ModifiedAt=SYSUTCDATETIME(), ModifiedBy=@ReversedBy WHERE Id=@VoucherId;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END");

            migrationBuilder.Sql(@"
CREATE PROCEDURE dbo.usp_Accounting_TrialBalance
    @FromDate date,
    @ToDate date,
    @ProjectId bigint=NULL,
    @CostCenterId bigint=NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @ToDate<@FromDate THROW 51020, 'To date cannot be earlier than from date.', 1;
    SELECT a.Id AS LedgerAccountId,a.Code,a.Name AS AccountName,a.AccountType,
        CAST(ISNULL(SUM(CASE WHEN v.Id IS NOT NULL THEN l.Debit ELSE 0 END),0) AS decimal(18,2)) AS Debit,
        CAST(ISNULL(SUM(CASE WHEN v.Id IS NOT NULL THEN l.Credit ELSE 0 END),0) AS decimal(18,2)) AS Credit,
        CAST(ISNULL(SUM(CASE WHEN v.Id IS NOT NULL THEN l.Debit-l.Credit ELSE 0 END),0) AS decimal(18,2)) AS Balance
    FROM dbo.LedgerAccounts a
    LEFT JOIN dbo.JournalVoucherLines l ON l.LedgerAccountId=a.Id AND l.IsDeleted=0
    LEFT JOIN dbo.JournalVouchers v ON v.Id=l.JournalVoucherId AND v.IsDeleted=0 AND v.[Status] IN (2,3)
        AND v.VoucherDate>=@FromDate AND v.VoucherDate<DATEADD(day,1,@ToDate)
        AND (@ProjectId IS NULL OR v.ProjectId=@ProjectId)
        AND (@CostCenterId IS NULL OR v.CostCenterId=@CostCenterId)
    WHERE a.IsDeleted=0 AND a.IsActive=1 AND a.AllowPosting=1
    GROUP BY a.Id,a.Code,a.Name,a.AccountType
    HAVING ISNULL(SUM(CASE WHEN v.Id IS NOT NULL THEN l.Debit ELSE 0 END),0)<>0
        OR ISNULL(SUM(CASE WHEN v.Id IS NOT NULL THEN l.Credit ELSE 0 END),0)<>0
    ORDER BY a.Code;
END");

            migrationBuilder.Sql(@"
INSERT dbo.LedgerAccounts(Code,Name,AccountType,AllowPosting,IsActive,[Guid],CreatedAt,CreatedBy,IsDeleted)
SELECT v.Code,v.Name,v.AccountType,1,1,NEWID(),SYSUTCDATETIME(),'migration',0
FROM (VALUES ('1000','Cash and Bank',1),('1100','Accounts Receivable',1),('1200','Inventory',1),
('1300','Employee Advances',1),('2000','Supplier Payable',2),('2100','Contractor Payable',2),
('2200','VAT and AIT Payable',2),('3000','Owner Equity',3),('4000','Property Sales Revenue',4),
('5000','Construction Cost',5),('5100','Salary Expense',5),('5200','Administrative Expense',5)) v(Code,Name,AccountType)
WHERE NOT EXISTS(SELECT 1 FROM dbo.LedgerAccounts a WHERE a.Code=v.Code);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF OBJECT_ID('dbo.usp_Accounting_TrialBalance','P') IS NOT NULL DROP PROCEDURE dbo.usp_Accounting_TrialBalance");
            migrationBuilder.Sql("IF OBJECT_ID('dbo.usp_Accounting_ReverseJournalVoucher','P') IS NOT NULL DROP PROCEDURE dbo.usp_Accounting_ReverseJournalVoucher");
            migrationBuilder.Sql("IF OBJECT_ID('dbo.usp_Accounting_PostJournalVoucher','P') IS NOT NULL DROP PROCEDURE dbo.usp_Accounting_PostJournalVoucher");
            migrationBuilder.DropTable(
                name: "JournalVoucherLines");

            migrationBuilder.DropTable(
                name: "JournalVouchers");

            migrationBuilder.DropTable(
                name: "LedgerAccounts");
        }
    }
}
