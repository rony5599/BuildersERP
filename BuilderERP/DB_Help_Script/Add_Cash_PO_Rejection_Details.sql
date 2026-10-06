/*
    Adds rejection/return-for-revision fields for:
      - Cash Requisitions
      - Cash Purchase Orders
      - Cash PO Bills

    Safe to run more than once. Existing columns are left unchanged.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.CashRequisitions', N'U') IS NULL
        THROW 50001, 'Table dbo.CashRequisitions does not exist.', 1;

    IF OBJECT_ID(N'dbo.CashPurchaseOrders', N'U') IS NULL
        THROW 50002, 'Table dbo.CashPurchaseOrders does not exist.', 1;

    IF OBJECT_ID(N'dbo.CashPoBills', N'U') IS NULL
        THROW 50003, 'Table dbo.CashPoBills does not exist.', 1;

    IF COL_LENGTH(N'dbo.CashRequisitions', N'RejectionReason') IS NULL
        ALTER TABLE dbo.CashRequisitions ADD RejectionReason nvarchar(1000) NULL;

    IF COL_LENGTH(N'dbo.CashRequisitions', N'RejectedBy') IS NULL
        ALTER TABLE dbo.CashRequisitions ADD RejectedBy nvarchar(max) NULL;

    IF COL_LENGTH(N'dbo.CashRequisitions', N'RejectedAt') IS NULL
        ALTER TABLE dbo.CashRequisitions ADD RejectedAt datetime2 NULL;

    IF COL_LENGTH(N'dbo.CashPurchaseOrders', N'RejectionReason') IS NULL
        ALTER TABLE dbo.CashPurchaseOrders ADD RejectionReason nvarchar(1000) NULL;

    IF COL_LENGTH(N'dbo.CashPurchaseOrders', N'RejectedBy') IS NULL
        ALTER TABLE dbo.CashPurchaseOrders ADD RejectedBy nvarchar(max) NULL;

    IF COL_LENGTH(N'dbo.CashPurchaseOrders', N'RejectedAt') IS NULL
        ALTER TABLE dbo.CashPurchaseOrders ADD RejectedAt datetime2 NULL;

    IF COL_LENGTH(N'dbo.CashPoBills', N'RejectionReason') IS NULL
        ALTER TABLE dbo.CashPoBills ADD RejectionReason nvarchar(1000) NULL;

    IF COL_LENGTH(N'dbo.CashPoBills', N'RejectedBy') IS NULL
        ALTER TABLE dbo.CashPoBills ADD RejectedBy nvarchar(max) NULL;

    IF COL_LENGTH(N'dbo.CashPoBills', N'RejectedAt') IS NULL
        ALTER TABLE dbo.CashPoBills ADD RejectedAt datetime2 NULL;

    COMMIT TRANSACTION;
    PRINT 'Cash requisition, Cash PO, and Cash PO bill rejection columns are ready.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
