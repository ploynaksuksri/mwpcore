DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[Submissions]') AND [c].[name] = N'DeleterId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[Submissions] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [mwp].[Submissions] DROP COLUMN [DeleterId];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[Submissions]') AND [c].[name] = N'DeletionTime');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[Submissions] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [mwp].[Submissions] DROP COLUMN [DeletionTime];

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[Submissions]') AND [c].[name] = N'IsDeleted');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[Submissions] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [mwp].[Submissions] DROP COLUMN [IsDeleted];

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[Forms]') AND [c].[name] = N'DeleterId');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[Forms] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [mwp].[Forms] DROP COLUMN [DeleterId];

GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[Forms]') AND [c].[name] = N'DeletionTime');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[Forms] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [mwp].[Forms] DROP COLUMN [DeletionTime];

GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[Forms]') AND [c].[name] = N'IsDeleted');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[Forms] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [mwp].[Forms] DROP COLUMN [IsDeleted];

GO

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20201005134202_Add_SoftDelete_Form_And_Submission';

GO

