IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201104094052_PDFTron')
BEGIN
    DROP TABLE mwp.PdfAnnotations;
END;

GO