SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH(N'dbo.BoqHeaders', N'ApprovedAt') IS NULL
    ALTER TABLE [dbo].[BoqHeaders] ADD [ApprovedAt] datetime2 NULL;

IF COL_LENGTH(N'dbo.BoqHeaders', N'ApprovedBy') IS NULL
    ALTER TABLE [dbo].[BoqHeaders] ADD [ApprovedBy] nvarchar(max) NULL;

IF COL_LENGTH(N'dbo.BoqHeaders', N'RejectedAt') IS NULL
    ALTER TABLE [dbo].[BoqHeaders] ADD [RejectedAt] datetime2 NULL;

IF COL_LENGTH(N'dbo.BoqHeaders', N'RejectedBy') IS NULL
    ALTER TABLE [dbo].[BoqHeaders] ADD [RejectedBy] nvarchar(max) NULL;

IF COL_LENGTH(N'dbo.BoqHeaders', N'RejectionReason') IS NULL
    ALTER TABLE [dbo].[BoqHeaders] ADD [RejectionReason] nvarchar(1000) NULL;

IF COL_LENGTH(N'dbo.BoqHeaders', N'Status') IS NULL
BEGIN
    ALTER TABLE [dbo].[BoqHeaders]
        ADD [Status] nvarchar(30) NOT NULL
            CONSTRAINT [DF_BoqHeaders_Status] DEFAULT N'Draft';
END;

IF OBJECT_ID(N'[dbo].[BoqActionAssignments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BoqActionAssignments]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [CanDraftEdit] bit NOT NULL CONSTRAINT [DF_BoqActionAssignments_CanDraftEdit] DEFAULT 0,
        [CanSubmit] bit NOT NULL CONSTRAINT [DF_BoqActionAssignments_CanSubmit] DEFAULT 0,
        [CanRequestApproval] bit NOT NULL CONSTRAINT [DF_BoqActionAssignments_CanRequestApproval] DEFAULT 0,
        [CanApprove] bit NOT NULL CONSTRAINT [DF_BoqActionAssignments_CanApprove] DEFAULT 0,
        [CanReject] bit NOT NULL CONSTRAINT [DF_BoqActionAssignments_CanReject] DEFAULT 0,
        [CanCancel] bit NOT NULL CONSTRAINT [DF_BoqActionAssignments_CanCancel] DEFAULT 0,
        [Guid] uniqueidentifier NOT NULL CONSTRAINT [DF_BoqActionAssignments_Guid] DEFAULT NEWID(),
        [CreatedAt] datetime2 NOT NULL CONSTRAINT [DF_BoqActionAssignments_CreatedAt] DEFAULT SYSUTCDATETIME(),
        [CreatedBy] nvarchar(max) NULL,
        [ModifiedAt] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL CONSTRAINT [DF_BoqActionAssignments_IsDeleted] DEFAULT 0,
        CONSTRAINT [PK_BoqActionAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BoqActionAssignments_AspNetUsers_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE [name] = N'IX_BoqActionAssignments_Guid'
      AND [object_id] = OBJECT_ID(N'[dbo].[BoqActionAssignments]'))
BEGIN
    CREATE UNIQUE INDEX [IX_BoqActionAssignments_Guid]
        ON [dbo].[BoqActionAssignments]([Guid]);
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE [name] = N'IX_BoqActionAssignments_UserId'
      AND [object_id] = OBJECT_ID(N'[dbo].[BoqActionAssignments]'))
BEGIN
    CREATE UNIQUE INDEX [IX_BoqActionAssignments_UserId]
        ON [dbo].[BoqActionAssignments]([UserId]);
END;

IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NOT NULL
   AND NOT EXISTS (
       SELECT 1
       FROM [dbo].[__EFMigrationsHistory]
       WHERE [MigrationId] = N'20261008091052_AddBoqApprovalWorkflow')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261008091052_AddBoqApprovalWorkflow', N'8.0.19');
END;

COMMIT TRANSACTION;
