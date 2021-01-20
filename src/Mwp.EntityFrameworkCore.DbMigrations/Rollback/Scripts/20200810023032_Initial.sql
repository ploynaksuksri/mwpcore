IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpAuditLogActions];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpBackgroundJobs];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpClaimTypes];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpEntityPropertyChanges];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpFeatureValues];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpLanguages];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpLanguageTexts];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpOrganizationUnitRoles];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpPermissionGrants];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpRoleClaims];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpSecurityLogs];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpSettings];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpTextTemplateContents];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpUserClaims];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpUserLogins];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpUserOrganizationUnits];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpUserRoles];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpUserTokens];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerApiClaims];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerApiScopeClaims];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerApiSecrets];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerClientClaims];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerClientCorsOrigins];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerClientGrantTypes];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerClientIdPRestrictions];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerClientPostLogoutRedirectUris];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerClientProperties];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerClientRedirectUris];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerClientScopes];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerClientSecrets];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerDeviceFlowCodes];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerIdentityClaims];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerPersistedGrants];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [SaasEditions];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [SaasTenantConnectionStrings];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpEntityChanges];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpOrganizationUnits];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpRoles];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpUsers];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerApiScopes];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerClients];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerIdentityResources];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [SaasTenants];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [AbpAuditLogs];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DROP TABLE [IdentityServerApiResources];
END;

GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200506145520_Initial')
BEGIN
    DELETE FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20200506145520_Initial';
END;

GO

