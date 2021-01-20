
IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200925015729_QBO')
BEGIN
    DROP TABLE mwp.QboTokens;
END;

GO


IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200925015729_QBO')
BEGIN
    DROP TABLE mwp.QboTenants;
END;

GO