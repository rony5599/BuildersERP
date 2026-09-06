/*
    Cash Purchase Order + Cash Purchase Order Detail
    Manual T-SQL script (for use while the Web project's build output is locked
    by an active debug session, blocking `dotnet ef migrations add`).

    Table/column/constraint/index names match what EF Core would generate from
    CashPurchaseOrder / CashPurchaseOrderDetail via the fluent config in
    AppDbContext.cs, so a later `dotnet ef migrations add AddCashPurchaseOrder`
    should produce a migration whose Up() is a no-op against a database this
    script has already been run on (see note at the bottom).
*/

SET NOCOUNT ON;
GO

IF OBJECT_ID(N'[dbo].[CashPurchaseOrders]', N'U') IS NOT NULL
BEGIN
    RAISERROR('Table CashPurchaseOrders already exists. Aborting.', 16, 1);
    RETURN;
END
GO

CREATE TABLE [dbo].[CashPurchaseOrders] (
    [Id]                bigint IDENTITY(1,1) NOT NULL,
    [CPONumber]         nvarchar(max) NOT NULL,
    [OrderDate]         datetime2 NOT NULL,
    [TotalAmount]       decimal(18,2) NOT NULL,
    [ReceivedAmount]    decimal(18,2) NOT NULL,
    [DeliveryDate]      datetime2 NOT NULL,
    [Status]            int NOT NULL,
    [IsActive]          bit NOT NULL,
    [TermsOfPayment]    nvarchar(max) NULL,
    [DispatchedThrough] nvarchar(max) NULL,
    [Destination]       nvarchar(max) NULL,
    [Remarks]           nvarchar(max) NULL,
    [SupplierId]        bigint NOT NULL,
    [CashRequisitionId] bigint NOT NULL,
    [Guid]              uniqueidentifier NOT NULL,
    [CreatedAt]         datetime2 NOT NULL,
    [CreatedBy]         nvarchar(max) NULL,
    [ModifiedAt]        datetime2 NULL,
    [ModifiedBy]        nvarchar(max) NULL,
    [IsDeleted]         bit NOT NULL,
    CONSTRAINT [PK_CashPurchaseOrders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CashPurchaseOrders_Suppliers_SupplierId] FOREIGN KEY ([SupplierId])
        REFERENCES [dbo].[Suppliers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CashPurchaseOrders_CashRequisitions_CashRequisitionId] FOREIGN KEY ([CashRequisitionId])
        REFERENCES [dbo].[CashRequisitions] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_CashPurchaseOrders_Guid] ON [dbo].[CashPurchaseOrders] ([Guid]);
CREATE INDEX [IX_CashPurchaseOrders_SupplierId] ON [dbo].[CashPurchaseOrders] ([SupplierId]);
CREATE INDEX [IX_CashPurchaseOrders_CashRequisitionId] ON [dbo].[CashPurchaseOrders] ([CashRequisitionId]);
GO

IF OBJECT_ID(N'[dbo].[CashPurchaseOrderDetails]', N'U') IS NOT NULL
BEGIN
    RAISERROR('Table CashPurchaseOrderDetails already exists. Aborting.', 16, 1);
    RETURN;
END
GO

CREATE TABLE [dbo].[CashPurchaseOrderDetails] (
    [Id]                  bigint IDENTITY(1,1) NOT NULL,
    [OrderedQuantity]     decimal(18,3) NOT NULL,
    [UnitOfMeasure]       int NOT NULL,
    [UnitPrice]           decimal(18,2) NOT NULL,
    [DiscountPercent]     decimal(5,2) NOT NULL,
    [DiscountAmount]      decimal(18,2) NOT NULL,
    [VatPercent]          decimal(5,2) NOT NULL,
    [VatAmount]           decimal(18,2) NOT NULL,
    [TaxPercent]          decimal(5,2) NOT NULL,
    [TaxAmount]           decimal(18,2) NOT NULL,
    [LineTotal]           decimal(18,2) NOT NULL,
    [ReceivedQuantity]    decimal(18,3) NOT NULL,
    [CashPurchaseOrderId] bigint NOT NULL,
    [MaterialId]          bigint NOT NULL,
    [Guid]                uniqueidentifier NOT NULL,
    [CreatedAt]           datetime2 NOT NULL,
    [CreatedBy]           nvarchar(max) NULL,
    [ModifiedAt]          datetime2 NULL,
    [ModifiedBy]          nvarchar(max) NULL,
    [IsDeleted]           bit NOT NULL,
    CONSTRAINT [PK_CashPurchaseOrderDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CashPurchaseOrderDetails_CashPurchaseOrders_CashPurchaseOrderId] FOREIGN KEY ([CashPurchaseOrderId])
        REFERENCES [dbo].[CashPurchaseOrders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CashPurchaseOrderDetails_Materials_MaterialId] FOREIGN KEY ([MaterialId])
        REFERENCES [dbo].[Materials] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_CashPurchaseOrderDetails_Guid] ON [dbo].[CashPurchaseOrderDetails] ([Guid]);
CREATE INDEX [IX_CashPurchaseOrderDetails_CashPurchaseOrderId] ON [dbo].[CashPurchaseOrderDetails] ([CashPurchaseOrderId]);
CREATE INDEX [IX_CashPurchaseOrderDetails_MaterialId] ON [dbo].[CashPurchaseOrderDetails] ([MaterialId]);
GO

/*
    NOTE on EF migration history:
    This script does not touch [dbo].[__EFMigrationsHistory]. Once the Web
    project's build output is no longer locked (stop the debug session in
    Visual Studio), run:

        dotnet ef migrations add AddCashPurchaseOrder --project src/BuilderERP.Infrastructure --startup-project src/BuilderERP.Web

    to scaffold the real migration file for source control. If you've already
    run this script against a database, either:
      (a) run that migration's Up() against a *different* (not-yet-updated)
          database, or
      (b) after scaffolding, manually insert a matching row into
          [dbo].[__EFMigrationsHistory] for the databases this script was
          already applied to, so `dotnet ef database update` treats it as
          already applied there.
*/
