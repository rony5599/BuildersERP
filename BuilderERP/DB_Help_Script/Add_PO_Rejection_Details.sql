/*
    Adds rejection/return-for-revision fields for:
      - Purchase Requisitions
      - Purchase Orders
      - PO Bills

    Safe to run more than once. Existing columns are left unchanged.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.PurchaseRequisitions', N'U') IS NULL
        THROW 50001, 'Table dbo.PurchaseRequisitions does not exist.', 1;

    IF OBJECT_ID(N'dbo.PurchaseOrders', N'U') IS NULL
        THROW 50002, 'Table dbo.PurchaseOrders does not exist.', 1;

    IF OBJECT_ID(N'dbo.PoBills', N'U') IS NULL
        THROW 50003, 'Table dbo.PoBills does not exist.', 1;

    IF COL_LENGTH(N'dbo.PurchaseRequisitions', N'RejectionReason') IS NULL
        ALTER TABLE dbo.PurchaseRequisitions ADD RejectionReason nvarchar(1000) NULL;

    IF COL_LENGTH(N'dbo.PurchaseRequisitions', N'RejectedBy') IS NULL
        ALTER TABLE dbo.PurchaseRequisitions ADD RejectedBy nvarchar(max) NULL;

    IF COL_LENGTH(N'dbo.PurchaseRequisitions', N'RejectedAt') IS NULL
        ALTER TABLE dbo.PurchaseRequisitions ADD RejectedAt datetime2 NULL;

    IF COL_LENGTH(N'dbo.PurchaseOrders', N'RejectionReason') IS NULL
        ALTER TABLE dbo.PurchaseOrders ADD RejectionReason nvarchar(1000) NULL;

    IF COL_LENGTH(N'dbo.PurchaseOrders', N'RejectedBy') IS NULL
        ALTER TABLE dbo.PurchaseOrders ADD RejectedBy nvarchar(max) NULL;

    IF COL_LENGTH(N'dbo.PurchaseOrders', N'RejectedAt') IS NULL
        ALTER TABLE dbo.PurchaseOrders ADD RejectedAt datetime2 NULL;

    IF COL_LENGTH(N'dbo.PoBills', N'RejectionReason') IS NULL
        ALTER TABLE dbo.PoBills ADD RejectionReason nvarchar(1000) NULL;

    IF COL_LENGTH(N'dbo.PoBills', N'RejectedBy') IS NULL
        ALTER TABLE dbo.PoBills ADD RejectedBy nvarchar(max) NULL;

    IF COL_LENGTH(N'dbo.PoBills', N'RejectedAt') IS NULL
        ALTER TABLE dbo.PoBills ADD RejectedAt datetime2 NULL;

    COMMIT TRANSACTION;
    PRINT 'PO requisition, PO, and PO bill rejection columns are ready.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
