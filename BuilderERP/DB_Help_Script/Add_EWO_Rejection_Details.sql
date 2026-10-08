/*
    Adds rejection/return-for-revision fields for:
      - EWO Requisitions
      - Engineer Work Orders
      - EWO Bills

    Safe to run more than once. Existing columns are left unchanged.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.EngineerWorkOrderRequisitions', N'U') IS NULL
        THROW 50001, 'Table dbo.EngineerWorkOrderRequisitions does not exist.', 1;

    IF OBJECT_ID(N'dbo.EngineerWorkOrders', N'U') IS NULL
        THROW 50002, 'Table dbo.EngineerWorkOrders does not exist.', 1;

    IF OBJECT_ID(N'dbo.EwoBills', N'U') IS NULL
        THROW 50003, 'Table dbo.EwoBills does not exist.', 1;

    IF COL_LENGTH(N'dbo.EngineerWorkOrderRequisitions', N'RejectionReason') IS NULL
        ALTER TABLE dbo.EngineerWorkOrderRequisitions ADD RejectionReason nvarchar(1000) NULL;

    IF COL_LENGTH(N'dbo.EngineerWorkOrderRequisitions', N'RejectedBy') IS NULL
        ALTER TABLE dbo.EngineerWorkOrderRequisitions ADD RejectedBy nvarchar(max) NULL;

    IF COL_LENGTH(N'dbo.EngineerWorkOrderRequisitions', N'RejectedAt') IS NULL
        ALTER TABLE dbo.EngineerWorkOrderRequisitions ADD RejectedAt datetime2 NULL;

    IF COL_LENGTH(N'dbo.EngineerWorkOrders', N'RejectionReason') IS NULL
        ALTER TABLE dbo.EngineerWorkOrders ADD RejectionReason nvarchar(1000) NULL;

    IF COL_LENGTH(N'dbo.EngineerWorkOrders', N'RejectedBy') IS NULL
        ALTER TABLE dbo.EngineerWorkOrders ADD RejectedBy nvarchar(max) NULL;

    IF COL_LENGTH(N'dbo.EngineerWorkOrders', N'RejectedAt') IS NULL
        ALTER TABLE dbo.EngineerWorkOrders ADD RejectedAt datetime2 NULL;

    IF COL_LENGTH(N'dbo.EwoBills', N'RejectionReason') IS NULL
        ALTER TABLE dbo.EwoBills ADD RejectionReason nvarchar(1000) NULL;

    IF COL_LENGTH(N'dbo.EwoBills', N'RejectedBy') IS NULL
        ALTER TABLE dbo.EwoBills ADD RejectedBy nvarchar(max) NULL;

    IF COL_LENGTH(N'dbo.EwoBills', N'RejectedAt') IS NULL
        ALTER TABLE dbo.EwoBills ADD RejectedAt datetime2 NULL;

    COMMIT TRANSACTION;
    PRINT 'EWO requisition, EWO, and EWO bill rejection columns are ready.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
