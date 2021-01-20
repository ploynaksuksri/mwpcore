ALTER TABLE [mwp].[TenantExes] DROP CONSTRAINT [FK_TenantExes_TenantTypes_TenantTypeId];

GO

DROP INDEX [IX_TenantExes_TenantTypeId] ON [mwp].[TenantExes];

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[TenantExes]') AND [c].[name] = N'TenantTypeId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[TenantExes] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [mwp].[TenantExes] DROP COLUMN [TenantTypeId];

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[CommunicationAddress];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[CommunicationEmail];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[CommunicationPhone];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[CommunicationWebsite];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Documents];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[EntityCommunication];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[EntityEntity];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[EntityGroupEntity];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[RelationshipTypeEntityType];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Templates];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[TitleAuthor];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[TitleCategories];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[WorkpaperComponent];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Addresses];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Emails];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Phones];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Websites];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Communications];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[EntityGroups];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[RelationshipTypes];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Authors];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Components];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Workpapers];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Folders];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Accounts];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Workbooks];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Ledgers];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Engagements];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Titles];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Entities];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[Publishers];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[EntityTypes];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DROP TABLE [mwp].[TenantTypes];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft')
BEGIN
    DELETE FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20201103013114_V2_Schema_First_Draft';
END;

GO
