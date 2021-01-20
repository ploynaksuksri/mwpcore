DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[mwp].[WopiFiles]') AND [c].[name] = N'FormId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [mwp].[WopiFiles] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [mwp].[WopiFiles] DROP COLUMN [FormId];

GO

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20201013183415_Add_Form_Id_to_WopiFile';

GO

