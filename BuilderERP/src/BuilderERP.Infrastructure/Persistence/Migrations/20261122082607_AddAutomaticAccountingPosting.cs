using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuilderERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAutomaticAccountingPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE dbo.usp_Accounting_PostSource
    @SourceType nvarchar(50),
    @SourceId bigint,
    @PostedBy nvarchar(256)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    DECLARE @StartedTransaction bit=0;
    IF @@TRANCOUNT=0
    BEGIN
        BEGIN TRANSACTION;
        SET @StartedTransaction=1;
    END

    BEGIN TRY
        IF @SourceType NOT IN ('PoBill','EwoBill','SupplierPayment')
            THROW 51100, 'Unsupported accounting source type.', 1;

        -- Idempotency is enforced under a serializable range lock. Workflow retries are safe.
        DECLARE @PostingSourceType nvarchar(50)=@SourceType;
        IF @SourceType<>'SupplierPayment' AND EXISTS(SELECT 1 FROM dbo.JournalVouchers WITH (UPDLOCK,HOLDLOCK)
                  WHERE SourceType=@SourceType AND SourceId=@SourceId AND IsDeleted=0)
        BEGIN
            IF @StartedTransaction=1 COMMIT TRANSACTION;
            RETURN;
        END
        IF @SourceType='SupplierPayment'
        BEGIN
            IF EXISTS(SELECT 1 FROM dbo.JournalVouchers WITH (UPDLOCK,HOLDLOCK)
                      WHERE SourceId=@SourceId AND (SourceType='SupplierPayment' OR SourceType LIKE 'SupplierPayment.Repost.%')
                        AND [Status]=2 AND IsDeleted=0)
            BEGIN
                IF @StartedTransaction=1 COMMIT TRANSACTION;
                RETURN;
            END
            DECLARE @PriorPaymentPostings int=(SELECT COUNT(*) FROM dbo.JournalVouchers WITH (UPDLOCK,HOLDLOCK)
                WHERE SourceId=@SourceId AND (SourceType='SupplierPayment' OR SourceType LIKE 'SupplierPayment.Repost.%') AND IsDeleted=0);
            IF @PriorPaymentPostings>0 SET @PostingSourceType='SupplierPayment.Repost.'+CONVERT(varchar(10),@PriorPaymentPostings);
        END

        DECLARE @Amount decimal(18,2), @VoucherDate date, @ProjectId bigint,
                @SourceNumber nvarchar(100), @DebitCode nvarchar(30), @CreditCode nvarchar(30),
                @Narration nvarchar(500);

        IF @SourceType='PoBill'
        BEGIN
            SELECT @Amount=b.TotalAmount,@VoucherDate=b.BillDate,@ProjectId=pr.ProjectId,
                   @SourceNumber=b.BillNumber,@DebitCode='5000',@CreditCode='2000',
                   @Narration='PO bill '+b.BillNumber
            FROM dbo.PoBills b
            JOIN dbo.PurchaseOrders po ON po.Id=b.PurchaseOrderId
            JOIN dbo.VendorQuotations vq ON vq.Id=po.VendorQuotationId
            JOIN dbo.Rfqs r ON r.Id=vq.RfqId
            JOIN dbo.PurchaseRequisitions pr ON pr.Id=r.PurchaseRequisitionId
            WHERE b.Id=@SourceId AND b.IsDeleted=0 AND b.IsActive=1 AND b.[Status]=1;
        END
        ELSE IF @SourceType='EwoBill'
        BEGIN
            SELECT @Amount=b.NetPayable,@VoucherDate=b.BillDate,@ProjectId=wr.ProjectId,
                   @SourceNumber=b.BillNumber,@DebitCode='5000',@CreditCode='2100',
                   @Narration='EWO bill '+b.BillNumber
            FROM dbo.EwoBills b
            JOIN dbo.EngineerWorkOrders wo ON wo.Id=b.EngineerWorkOrderId
            JOIN dbo.EngineerWorkOrderRequisitions wr ON wr.Id=wo.EngineerWorkOrderRequisitionId
            WHERE b.Id=@SourceId AND b.IsDeleted=0 AND b.IsActive=1 AND b.[Status]=1;
        END
        ELSE
        BEGIN
            SELECT @Amount=p.Amount,@VoucherDate=p.PaymentDate,
                   @ProjectId=COALESCE(pr.ProjectId,ewr.ProjectId),@SourceNumber=p.PaymentNumber,
                   @DebitCode=CASE WHEN p.EwoBillId IS NULL THEN '2000' ELSE '2100' END,
                   @CreditCode='1000',@Narration='Supplier payment '+p.PaymentNumber
            FROM dbo.SupplierPayments p
            LEFT JOIN dbo.PoBills pb ON pb.Id=p.PoBillId
            LEFT JOIN dbo.PurchaseOrders po ON po.Id=pb.PurchaseOrderId
            LEFT JOIN dbo.VendorQuotations vq ON vq.Id=po.VendorQuotationId
            LEFT JOIN dbo.Rfqs r ON r.Id=vq.RfqId
            LEFT JOIN dbo.PurchaseRequisitions pr ON pr.Id=r.PurchaseRequisitionId
            LEFT JOIN dbo.EwoBills eb ON eb.Id=p.EwoBillId
            LEFT JOIN dbo.EngineerWorkOrders ewo ON ewo.Id=eb.EngineerWorkOrderId
            LEFT JOIN dbo.EngineerWorkOrderRequisitions ewr ON ewr.Id=ewo.EngineerWorkOrderRequisitionId
            WHERE p.Id=@SourceId AND p.IsDeleted=0 AND p.IsActive=1;
        END

        IF @Amount IS NULL THROW 51101, 'The source is missing or is not eligible for accounting posting.', 1;
        IF @Amount<=0 THROW 51102, 'The accounting source amount must be greater than zero.', 1;
        IF @ProjectId IS NULL THROW 51103, 'The accounting source has no project.', 1;

        DECLARE @DebitAccountId bigint=(SELECT Id FROM dbo.LedgerAccounts
            WHERE Code=@DebitCode AND IsDeleted=0 AND IsActive=1 AND AllowPosting=1);
        DECLARE @CreditAccountId bigint=(SELECT Id FROM dbo.LedgerAccounts
            WHERE Code=@CreditCode AND IsDeleted=0 AND IsActive=1 AND AllowPosting=1);
        IF @DebitAccountId IS NULL OR @CreditAccountId IS NULL
            THROW 51104, 'Required posting accounts are missing or inactive.', 1;

        DECLARE @Prefix nvarchar(20)='JV-'+CONVERT(char(6),@VoucherDate,112)+'-';
        DECLARE @Sequence int=ISNULL((SELECT MAX(TRY_CONVERT(int,SUBSTRING(VoucherNumber,LEN(@Prefix)+1,10)))
            FROM dbo.JournalVouchers WITH (UPDLOCK,HOLDLOCK) WHERE VoucherNumber LIKE @Prefix+'%'),0)+1;
        DECLARE @VoucherNumber nvarchar(50)=@Prefix+RIGHT('00000'+CONVERT(varchar(10),@Sequence),5);
        DECLARE @Now datetime2=SYSUTCDATETIME(), @VoucherId bigint;

        INSERT dbo.JournalVouchers(VoucherNumber,VoucherDate,Narration,[Status],SourceType,SourceId,ProjectId,
            PostedAt,PostedBy,[Guid],CreatedAt,CreatedBy,IsDeleted)
        VALUES(@VoucherNumber,@VoucherDate,@Narration,2,@PostingSourceType,@SourceId,@ProjectId,
            @Now,@PostedBy,NEWID(),@Now,@PostedBy,0);
        SET @VoucherId=SCOPE_IDENTITY();

        INSERT dbo.JournalVoucherLines(JournalVoucherId,LedgerAccountId,Description,Debit,Credit,LineNumber,
            [Guid],CreatedAt,CreatedBy,IsDeleted)
        VALUES
            (@VoucherId,@DebitAccountId,@Narration,@Amount,0,1,NEWID(),@Now,@PostedBy,0),
            (@VoucherId,@CreditAccountId,@Narration,0,@Amount,2,NEWID(),@Now,@PostedBy,0);

        IF @StartedTransaction=1 COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END");

            migrationBuilder.Sql(@"
CREATE PROCEDURE dbo.usp_Accounting_AutoReverseSource
    @SourceType nvarchar(50),
    @SourceId bigint,
    @ReversalDate date,
    @Reason nvarchar(500),
    @ReversedBy nvarchar(256)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @VoucherId bigint;
    SELECT TOP(1) @VoucherId=Id FROM dbo.JournalVouchers WITH (UPDLOCK,HOLDLOCK)
    WHERE SourceId=@SourceId AND IsDeleted=0 AND [Status]=2
      AND (SourceType=@SourceType OR (@SourceType='SupplierPayment' AND SourceType LIKE 'SupplierPayment.Repost.%'))
    ORDER BY Id DESC;
    IF @VoucherId IS NULL THROW 51110, 'No posted journal exists for this source.', 1;
    DECLARE @ReversalVoucherId bigint;
    EXEC dbo.usp_Accounting_ReverseJournalVoucher @VoucherId,@ReversalDate,@Reason,@ReversedBy,@ReversalVoucherId OUTPUT;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF OBJECT_ID('dbo.usp_Accounting_AutoReverseSource','P') IS NOT NULL DROP PROCEDURE dbo.usp_Accounting_AutoReverseSource");
            migrationBuilder.Sql("IF OBJECT_ID('dbo.usp_Accounting_PostSource','P') IS NOT NULL DROP PROCEDURE dbo.usp_Accounting_PostSource");
        }
    }
}
