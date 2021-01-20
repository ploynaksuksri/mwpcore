using Microsoft.EntityFrameworkCore;
using Mwp.CloudService;
using Mwp.Communications;
using Mwp.Content;
using Mwp.Engagements;
using Mwp.Entities;
using Mwp.Financials;
using Mwp.Form;
using Mwp.PdfTron;
using Mwp.Qbo;
using Mwp.SharedResource;
using Mwp.Tenants;
using Mwp.Wopi;
using Mwp.Xero;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Mwp.EntityFrameworkCore
{
    public static class MwpDbContextModelCreatingExtensions
    {
        public static void ConfigureMwp(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(MwpConsts.DbTablePrefix + "YourEntities", MwpConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});

            //Tenant
            ConfigureCloudService(builder);
            ConfigureTenantResource(builder);
            ConfigureTenant(builder);

            //Features
            ConfigureForm(builder);
            ConfigureWopi(builder);
            ConfigureXero(builder);
            ConfigureQbo(builder);
            ConfigurePdfAnnotation(builder);

            //Schema
            ConfigureAccount(builder);
            ConfigureAddress(builder);
            ConfigureAuthor(builder);
            ConfigureCommunication(builder);
            ConfigureCommunicationAddress(builder);
            ConfigureCommunicationEmail(builder);
            ConfigureCommunicationPhone(builder);
            ConfigureCommunicationWebsite(builder);
            ConfigureComponent(builder);
            ConfigureDocument(builder);
            ConfigureEmail(builder);
            ConfigureEngagement(builder);
            ConfigureEntity(builder);
            ConfigureEntityCommunication(builder);
            ConfigureEntityEntity(builder);
            ConfigureEntityGroup(builder);
            ConfigureEntityGroupEntity(builder);
            ConfigureEntityType(builder);
            ConfigureFolder(builder);
            ConfigureLedger(builder);
            ConfigurePhone(builder);
            ConfigurePublisher(builder);
            ConfigureRelationshipType(builder);
            ConfigureRelationshipTypeEntityType(builder);
            ConfigureTemplate(builder);
            ConfigureTenantType(builder);
            ConfigureTitle(builder);
            ConfigureTitleAuthor(builder);
            ConfigureTitleCategory(builder);
            ConfigureWebsite(builder);
            ConfigureWorkbook(builder);
            ConfigureWorkpaper(builder);
            ConfigureWorkpaperComponent(builder);
        }

        static string GetTableName(string entityName)
        {
            return $"{MwpConsts.DbTablePrefix}{entityName}";
        }

        static void ConfigureCloudService(ModelBuilder builder)
        {
            builder.Entity<CloudServiceProvider>(b =>
            {
                b.ToTable(GetTableName("CloudServiceProviders"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.PlatformName).IsRequired().HasMaxLength(CloudServiceProviderConsts.MaxPlatformNameLength);
                b.Property(x => x.CompanyName).IsRequired().HasMaxLength(CloudServiceProviderConsts.MaxCompanyNameLength);
            });

            builder.Entity<CloudServiceLocation>(b =>
            {
                b.ToTable(GetTableName("CloudServiceLocations"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.LocationName).IsRequired().HasMaxLength(CloudServiceLocationConsts.MaxLocationNameLength);
            });

            builder.Entity<CloudServiceType>(b =>
            {
                b.ToTable(GetTableName("CloudServiceTypes"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.ServiceTypeName).IsRequired().HasMaxLength(CloudServiceTypeConsts.MaxServiceTypeNameLength);
            });

            builder.Entity<CloudService.CloudService>(b =>
            {
                b.ToTable(GetTableName("CloudServices"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.ServiceName).IsRequired().HasMaxLength(CloudServiceConsts.MaxServiceNameLength);

                b.HasOne(x => x.CloudServiceProvider).WithMany().HasForeignKey(x => x.CloudServiceProviderId);
                b.HasOne(x => x.CloudServiceType).WithMany().HasForeignKey(x => x.CloudServiceTypeId);
            });

            builder.Entity<CloudServiceOption>(b =>
            {
                b.ToTable(GetTableName("CloudServiceOptions"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.OptionName).IsRequired().HasMaxLength(CloudServiceOptionConsts.MaxOptionNameLength);
                b.Property(x => x.OptionDesc).IsRequired().HasMaxLength(CloudServiceOptionConsts.MaxOptionDescLength);

                b.HasOne(x => x.CloudService).WithMany().HasForeignKey(x => x.CloudServiceId);
            });

            builder.Entity<CloudService.SharedResource>(b =>
            {
                b.ToTable(GetTableName("SharedResources"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.SecretName).HasMaxLength(SharedResourceConsts.MaxSecretNameLength);

                b.HasOne(e => e.CloudServiceLocation).WithMany().HasForeignKey(e => e.CloudServiceLocationId);
                b.HasOne(e => e.CloudServiceOption).WithMany().HasForeignKey(e => e.CloudServiceOptionId);
            });
        }

        static void ConfigureTenantResource(ModelBuilder builder)
        {
            builder.Entity<TenantResource>(b =>
            {
                b.ToTable(GetTableName("TenantResources"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Name).HasMaxLength(TenantResourceConsts.MaxNameLength);
                b.Property(x => x.SubscriptionId).HasMaxLength(TenantResourceConsts.MaxSubscriptionIdLength);
                b.Property(x => x.ResourceGroup).HasMaxLength(TenantResourceConsts.MaxResourceGroupLength);
                b.Property(x => x.ConnectionString).HasMaxLength(TenantResourceConsts.MaxConnectionStringLength);
                b.Property(x => x.ServerName).HasMaxLength(TenantResourceConsts.MaxServerNameLength);
                b.Property(x => x.IsActive).HasDefaultValue(true);

                b.HasOne(e => e.CloudServiceOption).WithMany().HasForeignKey(e => e.CloudServiceOptionId);
                b.HasOne(e => e.CloudServiceLocation).WithMany().HasForeignKey(e => e.CloudServiceLocationId);
                b.HasOne(e => e.TenantEx).WithMany(tx => tx.TenantResources).HasForeignKey(e => e.TenantExId);
            });
        }

        static void ConfigureTenant(ModelBuilder builder)
        {
            builder.Entity<TenantEx>(b =>
            {
                b.ToTable(GetTableName("TenantExes"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.IsActive).HasDefaultValue(true);

                b.HasOne(tx => tx.Tenant).WithOne().HasForeignKey<TenantEx>(tx => tx.TenantId);
            });
        }

        static void ConfigureForm(ModelBuilder builder)
        {
            builder.Entity<Form.Form>(x =>
            {
                x.ToTable(GetTableName("Forms"), MwpConsts.DbSchema);
                x.ConfigureByConvention();

                x.Property(form => form.Name).IsRequired().HasMaxLength(FormConsts.MaxNameLength);
                x.Property(form => form.HierarchicalPath).HasMaxLength(FormConsts.MaxHierarchicalPathLength).HasColumnType($"varchar({FormConsts.MaxHierarchicalPathLength})");

                x.HasIndex(form => form.ParentId).HasName("IDX_FORM_PARENT_ID");
                x.HasIndex(form => form.HierarchicalPath).HasName("IDX_FORM_HIERARCHICAL_PATH");
            });

            builder.Entity<Submission>(x =>
            {
                x.ToTable(GetTableName("Submissions"), MwpConsts.DbSchema);
                x.ConfigureByConvention();

                x.Property(submission => submission.FormId).IsRequired();

                x.HasIndex(submission => submission.FormId).HasName("IDX_SUBMISSION_FORM_ID");
                x.HasOne(submission => submission.Form).WithMany().HasForeignKey(submission => submission.FormId).HasPrincipalKey(submission => submission.Id);
            });

            builder.Entity<UserFormConfig>(x =>
            {
                x.ToTable(GetTableName("UserFormConfigs"), MwpConsts.DbSchema);
                x.ConfigureByConvention();
            });
        }

        static void ConfigureWopi(ModelBuilder builder)
        {
            builder.Entity<WopiFile>(b =>
            {
                b.ToTable(GetTableName("WopiFiles"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(e => e.LockValue).HasMaxLength(WopiFileConsts.MaxLockValueLength);
            });

            builder.Entity<WopiFileHistory>(b =>
            {
                b.ToTable(GetTableName("WopiFileHistories"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(e => e.WopiFileId).IsRequired();
                b.Property(e => e.Version).IsRequired();
                b.Property(e => e.Revision).IsRequired();
                b.Property(e => e.FileIdInStorage).IsRequired();
                b.Property(e => e.Size).IsRequired();
                b.Property(e => e.BaseFileName).IsRequired().HasMaxLength(WopiFileHistoryConsts.MaxBaseFileNameLength);
                b.Property(x => x.LastModifiedUsers).HasMaxLength(WopiFileHistoryConsts.MaxLastModifiedUsersLength);
                b.Property(x => x.LastModifiedSessionId).HasMaxLength(WopiFileHistoryConsts.MaxLastModifiedSessionIdLength);

                b.HasIndex(e => e.WopiFileId).HasName("IDX_WOPIHISTORIES_WOPI_FILE_ID");
            });
        }

        static void ConfigureXero(ModelBuilder builder)
        {
            builder.Entity<XeroTenant>(b =>
            {
                b.ToTable(GetTableName("XeroTenants"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.XeroTenantId).IsRequired();
                b.Property(x => x.Name).HasMaxLength(XeroConsts.MaxTenantNameLength);

                b.HasIndex(e => e.MwpTenantId).HasName("IDX_XEROTENANTS_MWP_TENANT_ID");
            });

            builder.Entity<XeroToken>(b =>
            {
                b.ToTable(GetTableName("XeroTokens"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.AccessToken).HasMaxLength(XeroConsts.MaxAccessTokenLength);
                b.Property(x => x.IdToken).HasMaxLength(XeroConsts.MaxIdTokenLength);
                b.Property(x => x.RefreshToken).HasMaxLength(XeroConsts.MaxRefreshTokenLength);
                b.Property(x => x.ExpiresAtUtc).IsRequired();
                b.Property(x => x.MwpUserId).IsRequired();

                b.HasIndex(e => e.MwpUserId).HasName("IDX_XEROTOKENS_MWP_USER_ID");
            });

            builder.Entity<XeroConnection>(b =>
            {
                b.ToTable(GetTableName("XeroConnections"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasIndex(e => e.MwpUserId).HasName("IDX_XEROCONNECTIONS_MWP_USER_ID");
                b.HasOne(x => x.XeroTenant).WithMany().HasForeignKey(x => x.XeroTenantId).HasPrincipalKey(x => x.XeroTenantId);
            });
        }

        static void ConfigureAccount(ModelBuilder builder)
        {
            builder.Entity<Account>(b =>
            {
                b.ToTable(GetTableName("Accounts"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Name).IsRequired().HasMaxLength(AccountConsts.NameMaxLength);
                b.Property(x => x.FullName).IsRequired().HasMaxLength(AccountConsts.FullNameMaxLength);
                b.Property(x => x.EmailAddress).IsRequired().HasMaxLength(AccountConsts.EmailAddressMaxLength);
                b.Property(x => x.PhoneNumber).HasMaxLength(AccountConsts.PhoneNumberMaxLength);

                b.HasIndex(x => x.AccountId).HasName("IX_FK_AccountAccount");
                b.HasIndex(x => x.LedgerId).HasName("IX_FK_LedgerAccount");

                b.HasOne(x => x.AccountNavigation).WithMany(x => x.InverseAccountNavigation).HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_AccountAccount");
                b.HasOne(x => x.Ledger).WithMany(x => x.Accounts).HasForeignKey(x => x.LedgerId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_LedgerAccount");
            });
        }

        static void ConfigureAddress(ModelBuilder builder)
        {
            builder.Entity<Address>(b =>
            {
                b.ToTable(GetTableName("Addresses"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Address.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Address.Name)).IsRequired().HasMaxLength(AddressConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Address.IsActive));
            });
        }

        static void ConfigureAuthor(ModelBuilder builder)
        {
            builder.Entity<Author>(b =>
            {
                b.ToTable(GetTableName("Authors"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Author.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Author.Name)).IsRequired().HasMaxLength(AuthorConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Author.IsActive));
            });
        }

        static void ConfigureCommunication(ModelBuilder builder)
        {
            builder.Entity<Communication>(b =>
            {
                b.ToTable(GetTableName("Communications"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Communication.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Communication.Name)).IsRequired().HasMaxLength(CommunicationConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Communication.IsActive));
            });
        }

        static void ConfigureCommunicationAddress(ModelBuilder builder)
        {
            builder.Entity<CommunicationAddress>(b =>
            {
                b.ToTable(GetTableName("CommunicationAddress"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.CommunicationId).HasColumnName("CommunicationId");
                b.Property(x => x.AddressId).HasColumnName("AddressId");

                b.HasKey(x => new { x.CommunicationId, x.AddressId });

                b.HasIndex(x => x.AddressId).HasName("IX_FK_CommunicationAddress_Address");

                b.HasOne(x => x.Address).WithMany(x => x.CommunicationAddress).HasForeignKey(x => x.AddressId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CommunicationAddress_Address");
                b.HasOne(x => x.Communication).WithMany(x => x.CommunicationAddress).HasForeignKey(x => x.CommunicationId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CommunicationAddress_Communication");
            });
        }

        static void ConfigureCommunicationEmail(ModelBuilder builder)
        {
            builder.Entity<CommunicationEmail>(b =>
            {
                b.ToTable(GetTableName("CommunicationEmail"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.CommunicationId).HasColumnName("CommunicationId");
                b.Property(x => x.EmailId).HasColumnName("EmailId");

                b.HasKey(x => new { x.CommunicationId, x.EmailId });

                b.HasIndex(x => x.EmailId).HasName("IX_FK_CommunicationEmail_Email");

                b.HasOne(x => x.Communication).WithMany(x => x.CommunicationEmail).HasForeignKey(x => x.CommunicationId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CommunicationEmail_Communication");
                b.HasOne(x => x.Email).WithMany(x => x.CommunicationEmail).HasForeignKey(x => x.EmailId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CommunicationEmail_Email");
            });
        }

        static void ConfigureCommunicationPhone(ModelBuilder builder)
        {
            builder.Entity<CommunicationPhone>(b =>
            {
                b.ToTable(GetTableName("CommunicationPhone"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.CommunicationId).HasColumnName("CommunicationId");
                b.Property(x => x.PhoneId).HasColumnName("PhoneId");

                b.HasKey(x => new { x.CommunicationId, x.PhoneId });

                b.HasIndex(x => x.PhoneId).HasName("IX_FK_CommunicationPhone_Phone");

                b.HasOne(x => x.Communication).WithMany(x => x.CommunicationPhone).HasForeignKey(x => x.CommunicationId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CommunicationPhone_Communication");
                b.HasOne(x => x.Phone).WithMany(x => x.CommunicationPhone).HasForeignKey(x => x.PhoneId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CommunicationPhone_Phone");
            });
        }

        static void ConfigureCommunicationWebsite(ModelBuilder builder)
        {
            builder.Entity<CommunicationWebsite>(b =>
            {
                b.ToTable(GetTableName("CommunicationWebsite"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.CommunicationId).HasColumnName("CommunicationId");
                b.Property(x => x.WebsiteId).HasColumnName("WebsiteId");

                b.HasKey(x => new { x.CommunicationId, x.WebsiteId });

                b.HasIndex(x => x.WebsiteId).HasName("IX_FK_CommunicationWebsite_Website");

                b.HasOne(x => x.Communication).WithMany(x => x.CommunicationWebsite).HasForeignKey(x => x.CommunicationId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CommunicationWebsite_Communication");
                b.HasOne(x => x.Website).WithMany(x => x.CommunicationWebsite).HasForeignKey(x => x.WebsiteId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CommunicationWebsite_Website");
            });
        }

        static void ConfigureComponent(ModelBuilder builder)
        {
            builder.Entity<Component>(b =>
            {
                b.ToTable(GetTableName("Components"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Component.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Component.Name)).IsRequired().HasMaxLength(ComponentConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Component.IsActive));
            });
        }

        static void ConfigureDocument(ModelBuilder builder)
        {
            builder.Entity<Document>(b =>
            {
                b.ToTable(GetTableName("Documents"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Document.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Document.Name)).IsRequired().HasMaxLength(DocumentConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Document.IsActive));
                b.Property(x => x.WorkpaperId).HasColumnName(nameof(Document.WorkpaperId));

                b.HasOne(x => x.Workpaper).WithMany(x => x.Documents).HasForeignKey(x => x.WorkpaperId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WorkpaperDocument");
            });
        }

        static void ConfigureEmail(ModelBuilder builder)
        {
            builder.Entity<Email>(b =>
            {
                b.ToTable(GetTableName("Emails"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Email.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Email.Name)).IsRequired().HasMaxLength(EmailConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Email.IsActive));
            });
        }

        static void ConfigureEngagement(ModelBuilder builder)
        {
            builder.Entity<Engagement>(b =>
            {
                b.ToTable(GetTableName("Engagements"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Engagement.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Engagement.Name)).IsRequired().HasMaxLength(EngagementConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Engagement.IsActive));

                b.HasIndex(x => x.EntityId).HasName("IX_FK_EntityEngagement");

                b.HasOne(x => x.Entity).WithMany(x => x.Engagements).HasForeignKey(x => x.EntityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_EntityEngagement");
            });
        }

        static void ConfigureEntity(ModelBuilder builder)
        {
            builder.Entity<Entity>(b =>
            {
                b.ToTable(GetTableName("Entities"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Entity.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Entity.Name)).IsRequired().HasMaxLength(EntityConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Entity.IsActive));

                b.HasIndex(x => x.EntityTypeId).HasName("IX_FK_EntityEntityType");
                b.HasIndex(x => x.TenantExId).HasName("IX_FK_TenantExEntity");

                b.HasOne(x => x.EntityType).WithMany(p => p.Entities).HasForeignKey(x => x.EntityTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_EntityEntityType");
                b.HasOne(x => x.TenantEx).WithMany(p => p.Entities).HasForeignKey(x => x.TenantExId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_TenantExEntity");
            });
        }

        static void ConfigureEntityCommunication(ModelBuilder builder)
        {
            builder.Entity<EntityCommunication>(b =>
            {
                b.ToTable(GetTableName("EntityCommunication"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.EntityId).HasColumnName("EntityId");
                b.Property(x => x.CommunicationId).HasColumnName("CommunicationId");

                b.HasKey(x => new { x.EntityId, x.CommunicationId });

                b.HasIndex(x => x.CommunicationId).HasName("IX_FK_EntityCommunication_Communication");

                b.HasOne(x => x.Communication).WithMany(x => x.EntityCommunication).HasForeignKey(x => x.CommunicationId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_EntityCommunication_Communication");
                b.HasOne(x => x.Entity).WithMany(x => x.EntityCommunication).HasForeignKey(x => x.EntityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_EntityCommunication_Entity");
            });
        }

        static void ConfigureEntityEntity(ModelBuilder builder)
        {
            builder.Entity<EntityEntity>(b =>
            {
                b.ToTable(GetTableName("EntityEntity"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.MemberOfId).HasColumnName("MemberOfId");
                b.Property(x => x.MemberId).HasColumnName("MemberId");

                b.HasKey(x => new { x.MemberOfId, x.MemberId });

                b.HasIndex(x => x.MemberId).HasName("IX_FK_EntityEntity_Member");

                b.HasOne(x => x.MemberOf).WithMany(x => x.MembersOf).HasForeignKey(x => x.MemberOfId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_EntityEntity_MemberOf");
                b.HasOne(x => x.Member).WithMany(x => x.Members).HasForeignKey(x => x.MemberId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_EntityEntity_Member");
            });
        }

        static void ConfigureEntityGroup(ModelBuilder builder)
        {
            builder.Entity<EntityGroup>(b =>
            {
                b.ToTable(GetTableName("EntityGroups"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(EntityGroup.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(EntityGroup.Name)).IsRequired().HasMaxLength(EntityGroupConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(EntityGroup.IsActive));
            });
        }

        static void ConfigureEntityGroupEntity(ModelBuilder builder)
        {
            builder.Entity<EntityGroupEntity>(b =>
            {
                b.ToTable(GetTableName("EntityGroupEntity"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.EntityGroupId).HasColumnName("EntityGroupId");
                b.Property(x => x.EntityId).HasColumnName("EntityId");

                b.HasKey(x => new { x.EntityGroupId, x.EntityId });

                b.HasIndex(x => x.EntityId).HasName("IX_FK_EntityGroupEntity_Entity");

                b.HasOne(x => x.Entity).WithMany(x => x.EntityGroupEntity).HasForeignKey(x => x.EntityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_EntityGroupEntity_Entity");
                b.HasOne(x => x.EntityGroup).WithMany(x => x.EntityGroupEntity).HasForeignKey(x => x.EntityGroupId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_EntityGroupEntity_EntityGroup");
            });
        }

        static void ConfigureEntityType(ModelBuilder builder)
        {
            builder.Entity<EntityType>(b =>
            {
                b.ToTable(GetTableName("EntityTypes"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(EntityType.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(EntityType.Name)).IsRequired().HasMaxLength(EntityTypeConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(EntityType.IsActive));
            });
        }

        static void ConfigureFolder(ModelBuilder builder)
        {
            builder.Entity<Folder>(b =>
            {
                b.ToTable(GetTableName("Folders"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Folder.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Folder.Name)).IsRequired().HasMaxLength(FolderConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Folder.IsActive));

                b.HasIndex(x => x.ParentFolderId).HasName("IX_FK_ParentFolderFolder");
                b.HasIndex(x => x.WorkbookId).HasName("IX_FK_WorkbookFolder");

                b.HasOne(x => x.Parent).WithMany(x => x.Folders).HasForeignKey(x => x.ParentFolderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ParentFolderFolder");
                b.HasOne(x => x.Workbook).WithMany(x => x.Folders).HasForeignKey(x => x.WorkbookId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WorkbookFolder");
            });
        }

        static void ConfigureLedger(ModelBuilder builder)
        {
            builder.Entity<Ledger>(b =>
            {
                b.ToTable(GetTableName("Ledgers"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Ledger.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Ledger.Name)).IsRequired().HasMaxLength(LedgerConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Ledger.IsActive));

                b.HasIndex(x => x.EntityId).HasName("IX_FK_EntityLedger");

                // 1 to 0..1 relationship
                b.HasOne(x => x.Entity).WithMany(x => x.Ledger).HasForeignKey(x => x.EntityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_EntityLedger");
            });
        }

        static void ConfigurePhone(ModelBuilder builder)
        {
            builder.Entity<Phone>(b =>
            {
                b.ToTable(GetTableName("Phones"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Phone.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Phone.Name)).IsRequired().HasMaxLength(PhoneConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Phone.IsActive));
            });
        }

        static void ConfigurePublisher(ModelBuilder builder)
        {
            builder.Entity<Publisher>(b =>
            {
                b.ToTable(GetTableName("Publishers"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Publisher.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Publisher.Name)).IsRequired().HasMaxLength(PublisherConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Publisher.IsActive));
            });
        }

        static void ConfigureRelationshipType(ModelBuilder builder)
        {
            builder.Entity<RelationshipType>(b =>
            {
                b.ToTable(GetTableName("RelationshipTypes"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(RelationshipType.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(RelationshipType.Name)).IsRequired().HasMaxLength(RelationshipTypeConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(RelationshipType.IsActive));
            });
        }

        static void ConfigureRelationshipTypeEntityType(ModelBuilder builder)
        {
            builder.Entity<RelationshipTypeEntityType>(b =>
            {
                b.ToTable(GetTableName("RelationshipTypeEntityType"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.RelationshipTypeId).HasColumnName("RelationshipTypeId");
                b.Property(x => x.EntityTypeId).HasColumnName("EntityTypeId");

                b.HasKey(x => new { x.RelationshipTypeId, x.EntityTypeId });

                b.HasIndex(x => x.EntityTypeId).HasName("IX_FK_RelationshipTypeEntityType_EntityType");

                b.HasOne(x => x.EntityType).WithMany(x => x.RelationshipTypeEntityType).HasForeignKey(x => x.EntityTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_RelationshipTypeEntityType_EntityType");
                b.HasOne(x => x.RelationshipType).WithMany(x => x.RelationshipTypeEntityType).HasForeignKey(x => x.RelationshipTypeId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_RelationshipTypeEntityType_RelationshipType");
            });
        }

        static void ConfigureTemplate(ModelBuilder builder)
        {
            builder.Entity<Template>(b =>
            {
                b.ToTable(GetTableName("Templates"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Template.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Template.Name)).IsRequired().HasMaxLength(TemplateConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Template.IsActive));
            });
        }

        static void ConfigureTenantType(ModelBuilder builder)
        {
            builder.Entity<TenantType>(b =>
            {
                b.ToTable(GetTableName("TenantTypes"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(TenantType.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(TenantType.Name)).IsRequired().HasMaxLength(TenantTypeConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(TenantType.IsActive));
            });
        }

        static void ConfigureTitle(ModelBuilder builder)
        {
            builder.Entity<Title>(b =>
            {
                b.ToTable(GetTableName("Titles"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Title.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Title.Name)).IsRequired().HasMaxLength(TitleConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Title.IsActive));
                b.Property(x => x.TitleCategoryId).HasColumnName("TitleCategoryId");
                b.Property(x => x.WorkbookId).HasColumnName("WorkbookId");

                b.HasIndex(x => x.PublisherId).HasName("IX_FK_PublisherTitle");
                b.HasIndex(x => x.TitleCategoryId).HasName("IX_FK_TitleTitleCategory");
                b.HasIndex(x => x.WorkbookId).HasName("IX_FK_WorkbookTitle");

                b.HasOne(x => x.Publisher).WithMany(x => x.Titles).HasForeignKey(x => x.PublisherId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PublisherTitle");

                // 1 to 0..1 relationship
                b.HasOne(x => x.TitleCategory).WithMany(x => x.Title).HasForeignKey(x => x.TitleCategoryId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_TitleTitleCategory");
                b.HasOne(x => x.Workbook).WithMany(x => x.Title).HasForeignKey(x => x.WorkbookId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WorkbookTitle");
            });
        }

        static void ConfigureTitleAuthor(ModelBuilder builder)
        {
            builder.Entity<TitleAuthor>(b =>
            {
                b.ToTable(GetTableName("TitleAuthor"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TitleId).HasColumnName("TitleId");
                b.Property(x => x.AuthorId).HasColumnName("AuthorId");

                b.HasKey(x => new { x.TitleId, x.AuthorId });

                b.HasIndex(x => x.AuthorId).HasName("IX_FK_TitleAuthor_Author");

                b.HasOne(x => x.Author).WithMany(x => x.TitleAuthor).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_TitleAuthor_Author");
                b.HasOne(x => x.Title).WithMany(x => x.TitleAuthor).HasForeignKey(x => x.TitleId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_TitleAuthor_Title");
            });
        }

        static void ConfigureTitleCategory(ModelBuilder builder)
        {
            builder.Entity<TitleCategory>(b =>
            {
                b.ToTable(GetTableName("TitleCategories"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(TitleCategory.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(TitleCategory.Name)).IsRequired().HasMaxLength(TitleCategoryConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(TitleCategory.IsActive));
            });
        }

        static void ConfigureWebsite(ModelBuilder builder)
        {
            builder.Entity<Website>(b =>
            {
                b.ToTable(GetTableName("Websites"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Website.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Website.Name)).IsRequired().HasMaxLength(WebsiteConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Website.IsActive));
            });
        }

        static void ConfigureWorkbook(ModelBuilder builder)
        {
            builder.Entity<Workbook>(b =>
            {
                b.ToTable(GetTableName("Workbooks"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Workbook.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Workbook.Name)).IsRequired().HasMaxLength(WorkbookConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Workbook.IsActive));

                b.HasIndex(x => x.EngagementId).HasName("IX_FK_EngagementWorkbook");

                b.HasOne(x => x.Engagement).WithMany(x => x.Workbooks).HasForeignKey(x => x.EngagementId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_EngagementWorkbook");
            });
        }

        static void ConfigureWorkpaper(ModelBuilder builder)
        {
            builder.Entity<Workpaper>(b =>
            {
                b.ToTable(GetTableName("Workpapers"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.TenantId).HasColumnName(nameof(Workpaper.TenantId));
                b.Property(x => x.Name).HasColumnName(nameof(Workpaper.Name)).IsRequired().HasMaxLength(WorkpaperConsts.NameMaxLength);
                b.Property(x => x.IsActive).HasColumnName(nameof(Workpaper.IsActive));

                b.HasIndex(x => x.FolderId).HasName("IX_FK_FolderWorkpaper");

                b.HasOne(d => d.Folder).WithMany(p => p.Workpapers).HasForeignKey(d => d.FolderId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_FolderWorkpaper");
            });
        }

        static void ConfigureWorkpaperComponent(ModelBuilder builder)
        {
            builder.Entity<WorkpaperComponent>(b =>
            {
                b.ToTable(GetTableName("WorkpaperComponent"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.WorkpaperId).HasColumnName("WorkpaperId");
                b.Property(x => x.ComponentId).HasColumnName("ComponentId");

                b.HasKey(x => new { x.WorkpaperId, x.ComponentId });

                b.HasIndex(x => x.ComponentId).HasName("IX_FK_WorkpaperComponent_Component");

                b.HasOne(x => x.Component).WithMany(x => x.WorkpaperComponent).HasForeignKey(x => x.ComponentId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WorkpaperComponent_Component");
                b.HasOne(x => x.Workpaper).WithMany(x => x.WorkpaperComponent).HasForeignKey(x => x.WorkpaperId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_WorkpaperComponent_Workpaper");
            });
        }

        private static void ConfigureQbo(ModelBuilder builder)
        {
            builder.Entity<QboTenant>(b =>
            {
                b.ToTable(GetTableName("QboTenants"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.QboTenantId).IsRequired().HasMaxLength(QboConsts.MaxTenantIdLength);
                b.Property(x => x.Name).IsRequired().HasMaxLength(QboConsts.MaxTenantNameLength);

                b.HasIndex(x => x.MwpTenantId).HasName("IDX_QBOTENANTS_MWP_TENANT_ID");
            });

            builder.Entity<QboToken>(b =>
            {
                b.ToTable(GetTableName("QboTokens"), MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.QboTenantId).IsRequired().HasMaxLength(QboConsts.MaxTenantIdLength);
                b.Property(x => x.AccessToken).IsRequired().HasMaxLength(QboConsts.MaxAccessTokenLength);
                b.Property(x => x.ExpiresAtUtc).IsRequired();
                b.Property(x => x.RefreshToken).IsRequired().HasMaxLength(QboConsts.MaxRefreshTokenLength);
                b.Property(x => x.RefreshTokenExpiresAtUtc).IsRequired();
                b.Property(x => x.IdToken).HasMaxLength(QboConsts.MaxIdTokenLength);
                b.Property(x => x.MwpUserId).IsRequired();

                b.HasIndex(x => x.MwpUserId).HasName("IDX_QBOTOKENS_MWP_USER_ID");
            });
        }

        private static void ConfigurePdfAnnotation(ModelBuilder builder)
        {
            builder.Entity<PdfAnnotation>(b =>
            {
                b.ToTable(GetTableName("PdfAnnotations"), schema: MwpConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.FileId).IsRequired().HasMaxLength(PdfAnnotationConsts.FileIdMaxLength);
                b.Property(x => x.AnnotationId).IsRequired().HasMaxLength(PdfAnnotationConsts.AnnotationIdMaxLength);
                b.Property(x => x.Annotation).IsRequired();

                b.HasIndex(x => x.FileId).HasName("IDX_PDFANNOTATION_MWP_FILE_ID");
                b.HasIndex(x => x.AnnotationId).HasName("IDX_PDFANNOTATION_MWP_ANNOTATION_ID");
            });
        }
    }
}