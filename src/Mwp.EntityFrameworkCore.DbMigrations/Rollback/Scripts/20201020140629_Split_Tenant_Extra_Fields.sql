ALTER TABLE [mwp].[TenantResources] DROP CONSTRAINT [FK_TenantResources_TenantExes_TenantExId];

GO

DROP TABLE [mwp].[TenantExes];

GO

DROP INDEX [IX_TenantResources_TenantExId] ON [mwp].[TenantResources];

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[TenantResources]') AND [c].[name] = N'TenantExId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[TenantResources] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [mwp].[TenantResources] DROP COLUMN [TenantExId];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[TenantResources]') AND [c].[name] = N'IsActive');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[TenantResources] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [mwp].[TenantResources] ALTER COLUMN [IsActive] bit NOT NULL;

GO

ALTER TABLE [SaasTenants] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);

GO

ALTER TABLE [SaasTenants] ADD [TenantParentId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

GO

CREATE INDEX [IX_TenantResources_TenantId] ON [mwp].[TenantResources] ([TenantId]);

GO

ALTER TABLE [mwp].[TenantResources] ADD CONSTRAINT [FK_TenantResources_SaasTenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [SaasTenants] ([Id]) ON DELETE CASCADE;

GO

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20201020140629_Split_Tenant_Extra_Fields';

GO

