/*
    EWO Bill: Engineer Work Order payment heads, EWO bills (detail / heads /
    adjustments) and supplier payments against EWO bills.
    Equivalent to EF Core migration 20260929093125_AddEwoBill.

    Generated with `dotnet ef migrations script --idempotent`: every step is
    guarded by a check on [dbo].[__EFMigrationsHistory], and the script inserts
    the history row itself, so it is safe to re-run and a later
    `dotnet ef database update` treats the migration as applied.

    Changes to existing data:
      - SupplierPayments.PoBillId becomes NULLable (existing rows keep their value).
      - New column SupplierPayments.EwoBillId (NULL for all existing rows).
      - New check constraint CK_SupplierPayments_OneBill: exactly one of
        PoBillId / EwoBillId is set. Existing rows all have PoBillId, so they pass.

    Prerequisites: EngineerWorkOrders, EngineerWorkOrderDetails, Suppliers,
    Materials and SupplierPayments already exist (earlier migrations).
*/

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SupplierPayments]') AND [c].[name] = N'PoBillId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [SupplierPayments] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [SupplierPayments] ALTER COLUMN [PoBillId] bigint NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    ALTER TABLE [SupplierPayments] ADD [EwoBillId] bigint NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE TABLE [EngineerWorkOrderPaymentHeads] (
        [Id] bigint NOT NULL IDENTITY,
        [HeadName] nvarchar(100) NOT NULL,
        [Percent] decimal(5,2) NOT NULL,
        [SortOrder] int NOT NULL,
        [EngineerWorkOrderId] bigint NOT NULL,
        [Guid] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [ModifiedAt] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_EngineerWorkOrderPaymentHeads] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EngineerWorkOrderPaymentHeads_EngineerWorkOrders_EngineerWorkOrderId] FOREIGN KEY ([EngineerWorkOrderId]) REFERENCES [EngineerWorkOrders] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE TABLE [EwoBills] (
        [Id] bigint NOT NULL IDENTITY,
        [BillNumber] nvarchar(450) NOT NULL,
        [BillDate] datetime2 NOT NULL,
        [ContractorBillNumber] nvarchar(max) NULL,
        [MrrNumber] nvarchar(max) NULL,
        [Remarks] nvarchar(max) NULL,
        [Status] int NOT NULL,
        [IsActive] bit NOT NULL,
        [MeasuredAmount] decimal(18,2) NOT NULL,
        [CumulativePercent] decimal(5,2) NOT NULL,
        [CumulativeDue] decimal(18,2) NOT NULL,
        [PreviouslyCertified] decimal(18,2) NOT NULL,
        [CertifiedAmount] decimal(18,2) NOT NULL,
        [AdditionAmount] decimal(18,2) NOT NULL,
        [DeductionAmount] decimal(18,2) NOT NULL,
        [NetPayable] decimal(18,2) NOT NULL,
        [EngineerWorkOrderId] bigint NOT NULL,
        [RootWorkOrderId] bigint NOT NULL,
        [SupplierId] bigint NOT NULL,
        [Guid] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [ModifiedAt] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_EwoBills] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EwoBills_EngineerWorkOrders_EngineerWorkOrderId] FOREIGN KEY ([EngineerWorkOrderId]) REFERENCES [EngineerWorkOrders] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EwoBills_EngineerWorkOrders_RootWorkOrderId] FOREIGN KEY ([RootWorkOrderId]) REFERENCES [EngineerWorkOrders] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EwoBills_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE TABLE [EwoBillAdjustments] (
        [Id] bigint NOT NULL IDENTITY,
        [Type] int NOT NULL,
        [Description] nvarchar(200) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [EwoBillId] bigint NOT NULL,
        [Guid] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [ModifiedAt] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_EwoBillAdjustments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EwoBillAdjustments_EwoBills_EwoBillId] FOREIGN KEY ([EwoBillId]) REFERENCES [EwoBills] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE TABLE [EwoBillDetails] (
        [Id] bigint NOT NULL IDENTITY,
        [MeasuredQuantity] decimal(18,3) NOT NULL,
        [UnitOfMeasure] int NOT NULL,
        [Rate] decimal(18,2) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [EwoBillId] bigint NOT NULL,
        [EngineerWorkOrderDetailId] bigint NOT NULL,
        [MaterialId] bigint NOT NULL,
        [Guid] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [ModifiedAt] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_EwoBillDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EwoBillDetails_EngineerWorkOrderDetails_EngineerWorkOrderDetailId] FOREIGN KEY ([EngineerWorkOrderDetailId]) REFERENCES [EngineerWorkOrderDetails] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EwoBillDetails_EwoBills_EwoBillId] FOREIGN KEY ([EwoBillId]) REFERENCES [EwoBills] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_EwoBillDetails_Materials_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [Materials] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE TABLE [EwoBillHeads] (
        [Id] bigint NOT NULL IDENTITY,
        [HeadName] nvarchar(100) NOT NULL,
        [HeadPercent] decimal(5,2) NOT NULL,
        [ClaimPercent] decimal(5,2) NOT NULL,
        [EwoBillId] bigint NOT NULL,
        [EngineerWorkOrderPaymentHeadId] bigint NOT NULL,
        [Guid] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [ModifiedAt] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_EwoBillHeads] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EwoBillHeads_EngineerWorkOrderPaymentHeads_EngineerWorkOrderPaymentHeadId] FOREIGN KEY ([EngineerWorkOrderPaymentHeadId]) REFERENCES [EngineerWorkOrderPaymentHeads] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EwoBillHeads_EwoBills_EwoBillId] FOREIGN KEY ([EwoBillId]) REFERENCES [EwoBills] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_SupplierPayments_EwoBillId] ON [SupplierPayments] ([EwoBillId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    EXEC(N'ALTER TABLE [SupplierPayments] ADD CONSTRAINT [CK_SupplierPayments_OneBill] CHECK (([PoBillId] IS NOT NULL AND [EwoBillId] IS NULL) OR ([PoBillId] IS NULL AND [EwoBillId] IS NOT NULL))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EngineerWorkOrderPaymentHeads_EngineerWorkOrderId] ON [EngineerWorkOrderPaymentHeads] ([EngineerWorkOrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EngineerWorkOrderPaymentHeads_Guid] ON [EngineerWorkOrderPaymentHeads] ([Guid]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EwoBillAdjustments_EwoBillId] ON [EwoBillAdjustments] ([EwoBillId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EwoBillAdjustments_Guid] ON [EwoBillAdjustments] ([Guid]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EwoBillDetails_EngineerWorkOrderDetailId] ON [EwoBillDetails] ([EngineerWorkOrderDetailId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EwoBillDetails_EwoBillId] ON [EwoBillDetails] ([EwoBillId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EwoBillDetails_Guid] ON [EwoBillDetails] ([Guid]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EwoBillDetails_MaterialId] ON [EwoBillDetails] ([MaterialId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EwoBillHeads_EngineerWorkOrderPaymentHeadId] ON [EwoBillHeads] ([EngineerWorkOrderPaymentHeadId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EwoBillHeads_EwoBillId] ON [EwoBillHeads] ([EwoBillId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EwoBillHeads_Guid] ON [EwoBillHeads] ([Guid]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EwoBills_BillNumber] ON [EwoBills] ([BillNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EwoBills_EngineerWorkOrderId] ON [EwoBills] ([EngineerWorkOrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EwoBills_Guid] ON [EwoBills] ([Guid]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EwoBills_RootWorkOrderId] ON [EwoBills] ([RootWorkOrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    CREATE INDEX [IX_EwoBills_SupplierId] ON [EwoBills] ([SupplierId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    ALTER TABLE [SupplierPayments] ADD CONSTRAINT [FK_SupplierPayments_EwoBills_EwoBillId] FOREIGN KEY ([EwoBillId]) REFERENCES [EwoBills] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260929093125_AddEwoBill'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260929093125_AddEwoBill', N'8.0.19');
END;
GO

COMMIT;
GO

