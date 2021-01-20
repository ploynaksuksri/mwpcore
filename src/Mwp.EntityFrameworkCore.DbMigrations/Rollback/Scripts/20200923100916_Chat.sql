IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200923100916_Chat')
BEGIN
    DROP TABLE [ChatConversations];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200923100916_Chat')
BEGIN
    DROP TABLE [ChatUserMessages];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200923100916_Chat')
BEGIN
    DROP TABLE [ChatUsers];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200923100916_Chat')
BEGIN
    DROP TABLE [ChatMessages];
END;

GO
