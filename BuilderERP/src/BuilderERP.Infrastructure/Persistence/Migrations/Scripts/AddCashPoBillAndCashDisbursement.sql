/*
    Cash PO Bill + Cash PO Bill Detail + Cash Disbursement
    Manual T-SQL script for production, equivalent to EF Core migrations:
      - 20260928050812_AddCashPoBill
      - 20260928053139_AddCashDisbursement

    Table/column/constraint/index names match what EF Core generated from
    CashPoBill / CashPoBillDetail / CashDisbursement via the fluent config in
    AppDbContext.cs. This script also inserts the corresponding rows into
    [dbo].[__EFMigrationsHistory], so a later `dotnet ef database update`
    against this database sees both migrations as already applied and does
    nothing for them.

    Run this whole script in one execution (SQLCMD mode / GO batches). Wrap
    in a transaction at the deployment-tool level if you want an all-or-
    nothing apply; each CREATE TABLE step here already guards itself with an
    existence check and RAISERROR + RETURN, so a partial/duplicate run is
    caught rather than silently skipped or double-applied.
*/

SET NOCOUNT ON;
GO

-- ============================================================
-- 1. CashPoBills
-- ============================================================
IF OBJECT_ID(N'[dbo].[CashPoBills]', N'U') IS NOT NULL
BEGIN
    RAISERROR('Table CashPoBills already exists. Aborting.', 16, 1);
    RETURN;
END
GO

CREATE TABLE [dbo].[CashPoBills] (
    [Id]                  bigint IDENTITY(1,1) NOT NULL,
    [BillNumber]          nvarchar(450) NOT NULL,
    [BillDate]            datetime2 NOT NULL,
    [MemoNumber]          nvarchar(max) NULL,
    [TotalAmount]         decimal(18,2) NOT NULL,
    [Remarks]             nvarchar(max) NULL,
    [Status]              int NOT NULL,
    [IsActive]            bit NOT NULL,
    [CashPurchaseOrderId] bigint NOT NULL,
    [RequesterEmployeeId] bigint NOT NULL,
    [Guid]                uniqueidentifier NOT NULL,
    [CreatedAt]           datetime2 NOT NULL,
    [CreatedBy]           nvarchar(max) NULL,
    [ModifiedAt]          datetime2 NULL,
    [ModifiedBy]          nvarchar(max) NULL,
    [IsDeleted]           bit NOT NULL,
    CONSTRAINT [PK_CashPoBills] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CashPoBills_CashPurchaseOrders_CashPurchaseOrderId] FOREIGN KEY ([CashPurchaseOrderId])
        REFERENCES [dbo].[CashPurchaseOrders] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CashPoBills_Employees_RequesterEmployeeId] FOREIGN KEY ([RequesterEmployeeId])
        REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_CashPoBills_Guid] ON [dbo].[CashPoBills] ([Guid]);
CREATE INDEX [IX_CashPoBills_BillNumber] ON [dbo].[CashPoBills] ([BillNumber]);
CREATE INDEX [IX_CashPoBills_CashPurchaseOrderId] ON [dbo].[CashPoBills] ([CashPurchaseOrderId]);
CREATE INDEX [IX_CashPoBills_RequesterEmployeeId] ON [dbo].[CashPoBills] ([RequesterEmployeeId]);
GO

-- ============================================================
-- 2. CashPoBillDetails
-- ============================================================
IF OBJECT_ID(N'[dbo].[CashPoBillDetails]', N'U') IS NOT NULL
BEGIN
    RAISERROR('Table CashPoBillDetails already exists. Aborting.', 16, 1);
    RETURN;
END
GO

CREATE TABLE [dbo].[CashPoBillDetails] (
    [Id]                        bigint IDENTITY(1,1) NOT NULL,
    [BilledQuantity]            decimal(18,3) NOT NULL,
    [UnitOfMeasure]             int NOT NULL,
    [UnitPrice]                 decimal(18,2) NOT NULL,
    [DiscountPercent]           decimal(5,2) NOT NULL,
    [DiscountAmount]            decimal(18,2) NOT NULL,
    [VatPercent]                decimal(5,2) NOT NULL,
    [VatAmount]                 decimal(18,2) NOT NULL,
    [TaxPercent]                decimal(5,2) NOT NULL,
    [TaxAmount]                 decimal(18,2) NOT NULL,
    [LineTotal]                 decimal(18,2) NOT NULL,
    [CashPoBillId]              bigint NOT NULL,
    [CashPurchaseOrderDetailId] bigint NOT NULL,
    [MaterialId]                bigint NOT NULL,
    [Guid]                      uniqueidentifier NOT NULL,
    [CreatedAt]                 datetime2 NOT NULL,
    [CreatedBy]                 nvarchar(max) NULL,
    [ModifiedAt]                datetime2 NULL,
    [ModifiedBy]                nvarchar(max) NULL,
    [IsDeleted]                 bit NOT NULL,
    CONSTRAINT [PK_CashPoBillDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CashPoBillDetails_CashPoBills_CashPoBillId] FOREIGN KEY ([CashPoBillId])
        REFERENCES [dbo].[CashPoBills] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CashPoBillDetails_CashPurchaseOrderDetails_CashPurchaseOrderDetailId] FOREIGN KEY ([CashPurchaseOrderDetailId])
        REFERENCES [dbo].[CashPurchaseOrderDetails] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CashPoBillDetails_Materials_MaterialId] FOREIGN KEY ([MaterialId])
        REFERENCES [dbo].[Materials] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_CashPoBillDetails_Guid] ON [dbo].[CashPoBillDetails] ([Guid]);
CREATE INDEX [IX_CashPoBillDetails_CashPoBillId] ON [dbo].[CashPoBillDetails] ([CashPoBillId]);
CREATE INDEX [IX_CashPoBillDetails_CashPurchaseOrderDetailId] ON [dbo].[CashPoBillDetails] ([CashPurchaseOrderDetailId]);
CREATE INDEX [IX_CashPoBillDetails_MaterialId] ON [dbo].[CashPoBillDetails] ([MaterialId]);
GO

-- ============================================================
-- 3. CashDisbursements
-- ============================================================
IF OBJECT_ID(N'[dbo].[CashDisbursements]', N'U') IS NOT NULL
BEGIN
    RAISERROR('Table CashDisbursements already exists. Aborting.', 16, 1);
    RETURN;
END
GO

CREATE TABLE [dbo].[CashDisbursements] (
    [Id]                  bigint IDENTITY(1,1) NOT NULL,
    [DisbursementNumber]  nvarchar(450) NOT NULL,
    [DisbursementDate]    datetime2 NOT NULL,
    [Amount]              decimal(18,2) NOT NULL,
    [Method]              int NOT NULL,
    [ReferenceNumber]     nvarchar(max) NULL,
    [Remarks]             nvarchar(max) NULL,
    [IsActive]            bit NOT NULL,
    [CashRequisitionId]   bigint NOT NULL,
    [RequesterEmployeeId] bigint NOT NULL,
    [Guid]                uniqueidentifier NOT NULL,
    [CreatedAt]           datetime2 NOT NULL,
    [CreatedBy]           nvarchar(max) NULL,
    [ModifiedAt]          datetime2 NULL,
    [ModifiedBy]          nvarchar(max) NULL,
    [IsDeleted]           bit NOT NULL,
    CONSTRAINT [PK_CashDisbursements] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CashDisbursements_CashRequisitions_CashRequisitionId] FOREIGN KEY ([CashRequisitionId])
        REFERENCES [dbo].[CashRequisitions] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CashDisbursements_Employees_RequesterEmployeeId] FOREIGN KEY ([RequesterEmployeeId])
        REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_CashDisbursements_Guid] ON [dbo].[CashDisbursements] ([Guid]);
CREATE INDEX [IX_CashDisbursements_CashRequisitionId] ON [dbo].[CashDisbursements] ([CashRequisitionId]);
CREATE INDEX [IX_CashDisbursements_DisbursementNumber] ON [dbo].[CashDisbursements] ([DisbursementNumber]);
CREATE INDEX [IX_CashDisbursements_RequesterEmployeeId] ON [dbo].[CashDisbursements] ([RequesterEmployeeId]);
GO

-- ============================================================
-- 4. Mark the equivalent EF migrations as applied
-- ============================================================
IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260928050812_AddCashPoBill')
    BEGIN
        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES (N'20260928050812_AddCashPoBill', N'8.0.19');
    END

    IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260928053139_AddCashDisbursement')
    BEGIN
        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES (N'20260928053139_AddCashDisbursement', N'8.0.19');
    END
END
ELSE
BEGIN
    PRINT 'NOTE: [dbo].[__EFMigrationsHistory] not found — this database is not EF-managed yet. Tables were created, but no history row was inserted.';
END
GO

/*
    Prerequisites this script assumes already exist in the target database
    (created by earlier migrations, already applied to production):
      - dbo.CashPurchaseOrders, dbo.CashPurchaseOrderDetails  (AddCashPurchaseOrder)
      - dbo.CashRequisitions                                  (AddCashRequisition)
      - dbo.Employees                                         (AddEmployeeAndDesignation)
      - dbo.Materials
    If any FK target table above is missing, the corresponding CREATE TABLE
    step will fail with an FK-resolution error and nothing after it in that
    GO batch will have run — check `dotnet ef migrations list` against
    production before running this script.
*/
