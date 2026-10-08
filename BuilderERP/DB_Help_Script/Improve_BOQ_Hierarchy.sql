/*
    BuilderERP - BOQ master/detail database update
    Adds:    dbo.BoqHeaders, dbo.WorkGroups
    Alters:  dbo.BoqItems

    Prerequisite migration:
        20261006045829_AddEwoWorkflowRejectionDetails

    IMPORTANT: The original EF migration intentionally deletes all existing
    BoqItems because the previous rows cannot be mapped reliably to the new
    BOQ header/work-group model.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[__EFMigrationsHistory]
        WHERE [MigrationId] = N'20261007081022_ImproveBoqHierarchy'
    )
    BEGIN
        DECLARE @ConstraintName sysname;

        -- Remove obsolete BOQ item columns and their default constraints.
        DECLARE obsolete_constraints CURSOR LOCAL FAST_FORWARD FOR
            SELECT dc.[name]
            FROM sys.default_constraints dc
            INNER JOIN sys.columns c
                ON c.[object_id] = dc.[parent_object_id]
               AND c.[column_id] = dc.[parent_column_id]
            WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[BoqItems]')
              AND c.[name] IN (N'Category', N'IsActive', N'ItemCode');

        OPEN obsolete_constraints;
        FETCH NEXT FROM obsolete_constraints INTO @ConstraintName;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            EXEC(N'ALTER TABLE [dbo].[BoqItems] DROP CONSTRAINT [' + @ConstraintName + N'];');
            FETCH NEXT FROM obsolete_constraints INTO @ConstraintName;
        END;
        CLOSE obsolete_constraints;
        DEALLOCATE obsolete_constraints;

        ALTER TABLE [dbo].[BoqItems] DROP COLUMN [Category], [IsActive], [ItemCode];

        EXEC sp_rename N'[dbo].[BoqItems].[UnitOfMeasure]', N'Unit', N'COLUMN';
        EXEC sp_rename N'[dbo].[BoqItems].[Rate]', N'UnitRate', N'COLUMN';
        EXEC sp_rename N'[dbo].[BoqItems].[Description]', N'ItemDescription', N'COLUMN';
        EXEC sp_rename N'[dbo].[BoqItems].[Amount]', N'TotalAmount', N'COLUMN';

        ALTER TABLE [dbo].[BoqItems] ALTER COLUMN [Quantity] decimal(18,4) NOT NULL;
        ALTER TABLE [dbo].[BoqItems] ALTER COLUMN [Unit] nvarchar(20) NOT NULL;
        ALTER TABLE [dbo].[BoqItems] ALTER COLUMN [UnitRate] decimal(18,4) NOT NULL;

        ALTER TABLE [dbo].[BoqItems]
            ADD [BoqId] bigint NOT NULL CONSTRAINT [DF_BoqItems_BoqId] DEFAULT (0),
                [WorkGroupId] bigint NOT NULL CONSTRAINT [DF_BoqItems_WorkGroupId] DEFAULT (0);

        -- Replace the old stored amount with a persisted computed amount.
        SET @ConstraintName = NULL;
        SELECT @ConstraintName = dc.[name]
        FROM sys.default_constraints dc
        INNER JOIN sys.columns c
            ON c.[object_id] = dc.[parent_object_id]
           AND c.[column_id] = dc.[parent_column_id]
        WHERE dc.[parent_object_id] = OBJECT_ID(N'[dbo].[BoqItems]')
          AND c.[name] = N'TotalAmount';

        IF @ConstraintName IS NOT NULL
            EXEC(N'ALTER TABLE [dbo].[BoqItems] DROP CONSTRAINT [' + @ConstraintName + N'];');

        ALTER TABLE [dbo].[BoqItems] DROP COLUMN [TotalAmount];
        ALTER TABLE [dbo].[BoqItems]
            ADD [TotalAmount] AS (CONVERT(decimal(38,8), [Quantity] * [UnitRate])) PERSISTED;

        CREATE TABLE [dbo].[BoqHeaders]
        (
            [Id] bigint IDENTITY(1,1) NOT NULL,
            [ProjectId] bigint NOT NULL,
            [BoqName] nvarchar(255) NOT NULL,
            [VersionNumber] int NOT NULL CONSTRAINT [DF_BoqHeaders_VersionNumber] DEFAULT (1),
            [IsActive] bit NOT NULL CONSTRAINT [DF_BoqHeaders_IsActive] DEFAULT (1),
            [Guid] uniqueidentifier NOT NULL,
            [CreatedAt] datetime2 NOT NULL,
            [CreatedBy] nvarchar(max) NULL,
            [ModifiedAt] datetime2 NULL,
            [ModifiedBy] nvarchar(max) NULL,
            [IsDeleted] bit NOT NULL,
            CONSTRAINT [PK_BoqHeaders] PRIMARY KEY ([Id]),
            CONSTRAINT [FK_BoqHeaders_Projects_ProjectId]
                FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([Id]) ON DELETE CASCADE
        );

        CREATE TABLE [dbo].[WorkGroups]
        (
            [Id] bigint IDENTITY(1,1) NOT NULL,
            [ParentGroupId] bigint NULL,
            [GroupCode] nvarchar(50) NOT NULL,
            [GroupName] nvarchar(255) NOT NULL,
            [Guid] uniqueidentifier NOT NULL,
            [CreatedAt] datetime2 NOT NULL,
            [CreatedBy] nvarchar(max) NULL,
            [ModifiedAt] datetime2 NULL,
            [ModifiedBy] nvarchar(max) NULL,
            [IsDeleted] bit NOT NULL,
            CONSTRAINT [PK_WorkGroups] PRIMARY KEY ([Id]),
            CONSTRAINT [FK_WorkGroups_WorkGroups_ParentGroupId]
                FOREIGN KEY ([ParentGroupId]) REFERENCES [dbo].[WorkGroups] ([Id])
        );

        INSERT INTO [dbo].[WorkGroups]
            ([ParentGroupId], [GroupCode], [GroupName], [Guid], [CreatedAt], [IsDeleted])
        VALUES
            (NULL, N'03', N'Concrete', NEWID(), SYSUTCDATETIME(), 0),
            (NULL, N'04', N'Masonry', NEWID(), SYSUTCDATETIME(), 0),
            (NULL, N'05', N'Metals', NEWID(), SYSUTCDATETIME(), 0),
            (NULL, N'09', N'Finishes', NEWID(), SYSUTCDATETIME(), 0),
            (NULL, N'26', N'Electrical', NEWID(), SYSUTCDATETIME(), 0);

        INSERT INTO [dbo].[WorkGroups]
            ([ParentGroupId], [GroupCode], [GroupName], [Guid], [CreatedAt], [IsDeleted])
        VALUES
            ((SELECT [Id] FROM [dbo].[WorkGroups] WHERE [GroupCode] = N'03'), N'03.30', N'Cast-in-place Concrete', NEWID(), SYSUTCDATETIME(), 0),
            ((SELECT [Id] FROM [dbo].[WorkGroups] WHERE [GroupCode] = N'04'), N'04.20', N'Unit Masonry', NEWID(), SYSUTCDATETIME(), 0),
            ((SELECT [Id] FROM [dbo].[WorkGroups] WHERE [GroupCode] = N'05'), N'05.20', N'Metal Joists', NEWID(), SYSUTCDATETIME(), 0),
            ((SELECT [Id] FROM [dbo].[WorkGroups] WHERE [GroupCode] = N'09'), N'09.30', N'Tiling', NEWID(), SYSUTCDATETIME(), 0),
            ((SELECT [Id] FROM [dbo].[WorkGroups] WHERE [GroupCode] = N'09'), N'09.90', N'Painting', NEWID(), SYSUTCDATETIME(), 0),
            ((SELECT [Id] FROM [dbo].[WorkGroups] WHERE [GroupCode] = N'26'), N'26.10', N'Electrical Installation', NEWID(), SYSUTCDATETIME(), 0);

        INSERT INTO [dbo].[WorkGroups]
            ([ParentGroupId], [GroupCode], [GroupName], [Guid], [CreatedAt], [IsDeleted])
        VALUES
            ((SELECT [Id] FROM [dbo].[WorkGroups] WHERE [GroupCode] = N'03.30'), N'03.30.01', N'Foundation Concrete', NEWID(), SYSUTCDATETIME(), 0),
            ((SELECT [Id] FROM [dbo].[WorkGroups] WHERE [GroupCode] = N'03.30'), N'03.30.02', N'Column & Beam Concrete', NEWID(), SYSUTCDATETIME(), 0),
            ((SELECT [Id] FROM [dbo].[WorkGroups] WHERE [GroupCode] = N'04.20'), N'04.20.01', N'Brick Work', NEWID(), SYSUTCDATETIME(), 0),
            ((SELECT [Id] FROM [dbo].[WorkGroups] WHERE [GroupCode] = N'05.20'), N'05.20.01', N'Reinforcement', NEWID(), SYSUTCDATETIME(), 0);

        DELETE FROM [dbo].[BoqItems];

        ALTER TABLE [dbo].[BoqItems] DROP CONSTRAINT [FK_BoqItems_Projects_ProjectId];
        DROP INDEX [IX_BoqItems_ProjectId] ON [dbo].[BoqItems];
        ALTER TABLE [dbo].[BoqItems] DROP COLUMN [ProjectId];

        CREATE INDEX [IX_BoqItems_BoqId] ON [dbo].[BoqItems] ([BoqId]);
        CREATE INDEX [IX_BoqItems_WorkGroupId] ON [dbo].[BoqItems] ([WorkGroupId]);
        CREATE UNIQUE INDEX [IX_BoqHeaders_Guid] ON [dbo].[BoqHeaders] ([Guid]);
        CREATE INDEX [IX_BoqHeaders_ProjectId] ON [dbo].[BoqHeaders] ([ProjectId]) WHERE [IsActive] = 1;
        CREATE UNIQUE INDEX [IX_WorkGroups_GroupCode] ON [dbo].[WorkGroups] ([GroupCode]);
        CREATE UNIQUE INDEX [IX_WorkGroups_Guid] ON [dbo].[WorkGroups] ([Guid]);
        CREATE INDEX [IX_WorkGroups_ParentGroupId] ON [dbo].[WorkGroups] ([ParentGroupId]);

        ALTER TABLE [dbo].[BoqItems] ADD CONSTRAINT [FK_BoqItems_BoqHeaders_BoqId]
            FOREIGN KEY ([BoqId]) REFERENCES [dbo].[BoqHeaders] ([Id]) ON DELETE CASCADE;
        ALTER TABLE [dbo].[BoqItems] ADD CONSTRAINT [FK_BoqItems_WorkGroups_WorkGroupId]
            FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroups] ([Id]);

        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES (N'20261007081022_ImproveBoqHierarchy', N'8.0.19');
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[__EFMigrationsHistory]
        WHERE [MigrationId] = N'20261008090000_FinalizeBoqMasterDetail'
    )
    BEGIN
        ALTER TABLE [dbo].[BoqHeaders]
            ADD [ContingencyPercent] decimal(5,2) NOT NULL
                CONSTRAINT [DF_BoqHeaders_ContingencyPercent] DEFAULT (2);

        DROP INDEX [IX_BoqHeaders_ProjectId] ON [dbo].[BoqHeaders];
        CREATE UNIQUE INDEX [IX_BoqHeaders_ProjectId_BoqName_VersionNumber]
            ON [dbo].[BoqHeaders] ([ProjectId], [BoqName], [VersionNumber]);

        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES (N'20261008090000_FinalizeBoqMasterDetail', N'8.0.19');
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
