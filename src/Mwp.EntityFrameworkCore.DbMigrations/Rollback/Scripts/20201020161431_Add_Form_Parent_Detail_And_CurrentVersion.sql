DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[Forms]') AND [c].[name] = N'CurrentVersion');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[Forms] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [mwp].[Forms] DROP COLUMN [CurrentVersion];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[Forms]') AND [c].[name] = N'ParentDetail');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[Forms] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [mwp].[Forms] DROP COLUMN [ParentDetail];

GO

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20201020161431_Add_Form_Parent_Detail_And_CurrentVersion';

GO