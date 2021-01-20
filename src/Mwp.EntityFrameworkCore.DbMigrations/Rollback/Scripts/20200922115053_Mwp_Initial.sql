IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[SharedResources];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[Submissions];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[TenantResources];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[UserFormConfigs];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[WopiFileHistories];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[XeroConnections];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[XeroTokens];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[Forms];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[CloudServiceLocations];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[CloudServiceOptions];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[WopiFiles];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[XeroTenants];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[CloudServices];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[CloudServiceProviders];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DROP TABLE [mwp].[CloudServiceTypes];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SaasTenants]') AND [c].[name] = N'IsActive');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [SaasTenants] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [SaasTenants] DROP COLUMN [IsActive];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SaasTenants]') AND [c].[name] = N'TenantParentId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [SaasTenants] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [SaasTenants] DROP COLUMN [TenantParentId];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200910123839_Mwp_Initial')
BEGIN
    DELETE FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20200910123839_Mwp_Initial';
END;

GO

