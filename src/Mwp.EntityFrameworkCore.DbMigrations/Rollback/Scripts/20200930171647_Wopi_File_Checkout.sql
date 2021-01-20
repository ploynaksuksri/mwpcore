DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[WopiFiles]') AND [c].[name] = N'CheckoutBy');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[WopiFiles] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [mwp].[WopiFiles] DROP COLUMN [CheckoutBy];

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[WopiFiles]') AND [c].[name] = N'CheckoutTimestamp');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[WopiFiles] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [mwp].[WopiFiles] DROP COLUMN [CheckoutTimestamp];

GO

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20200930171647_Wopi_File_Checkout';

GO

