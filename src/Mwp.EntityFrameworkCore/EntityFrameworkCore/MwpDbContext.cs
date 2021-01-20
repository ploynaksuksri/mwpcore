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
using Mwp.Tenants;
using Mwp.Users;
using Mwp.Wopi;
using Mwp.Xero;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Users.EntityFrameworkCore;
using Volo.Chat.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;

namespace Mwp.EntityFrameworkCore
{
    /* This is your actual DbContext used on runtime.
     * It includes only your entities.
     * It does not include entities of the used modules, because each module has already
     * its own DbContext class. If you want to share some database tables with the used modules,
     * just create a structure like done for AppUser.
     *
     * Don't use this DbContext for database migrations since it does not contain tables of the
     * used modules (as explained above). See MwpMigrationsDbContext for migrations.
     */

    [ConnectionStringName("Default")]
    public class MwpDbContext : AbpDbContext<MwpDbContext>
    {
        public DbSet<AppUser> Users { get; set; }

        //CloudService
        public DbSet<CloudServiceProvider> CloudServiceProviders { get; set; }
        public DbSet<CloudServiceType> CloudServiceTypes { get; set; }
        public DbSet<CloudServiceLocation> CloudServiceLocations { get; set; }
        public DbSet<CloudServiceOption> CloudServiceOptions { get; set; }
        public DbSet<CloudService.CloudService> CloudServices { get; set; }
        public DbSet<CloudService.SharedResource> SharedResources { get; set; }

        //Tenant
        public DbSet<TenantResource> TenantResources { get; set; }
        public DbSet<TenantEx> TenantExes { get; set; }

        //Form
        public DbSet<Form.Form> Forms { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<UserFormConfig> UserFormConfigs { get; set; }

        //WOPI
        public DbSet<WopiFile> WopiFiles { get; set; }
        public DbSet<WopiFileHistory> WopiFileHistories { get; set; }

        #region Xero

        public DbSet<XeroTenant> XeroTenants { get; set; }
        public DbSet<XeroToken> XeroTokens { get; set; }
        public DbSet<XeroConnection> XeroConnections { get; set; }

        #endregion

        #region QBO

        public DbSet<QboTenant> QboTenants { get; set; }
        public DbSet<QboToken> QboTokens { get; set; }

        #endregion

        //Schema
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<CommunicationAddress> CommunicationAddress { get; set; }
        public DbSet<CommunicationEmail> CommunicationEmail { get; set; }
        public DbSet<CommunicationPhone> CommunicationPhone { get; set; }
        public DbSet<CommunicationWebsite> CommunicationWebsite { get; set; }
        public DbSet<Communication> Communications { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Engagement> Engagements { get; set; }
        public DbSet<EntityCommunication> EntityCommunication { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<EntityGroup> EntityGroups { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Ledger> Ledgers { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<RelationshipType> RelationshipTypes { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TenantType> TenantTypes { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<TitleAuthor> TitleAuthor { get; set; }
        public DbSet<TitleCategory> TitleCategories { get; set; }
        public DbSet<Website> Websites { get; set; }
        public DbSet<Workbook> Workbooks { get; set; }
        public DbSet<Workpaper> Workpapers { get; set; }
        public DbSet<WorkpaperComponent> WorkpaperComponent { get; set; }

        #region Pdf

        public DbSet<PdfAnnotation> PdfAnnotations { get; set; }

        #endregion Pdf

        /* Add DbSet properties for your Aggregate Roots / Entities here.
         * Also map them inside MwpDbContextModelCreatingExtensions.ConfigureMwp
         */

        public MwpDbContext(DbContextOptions<MwpDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /* Configure the shared tables (with included modules) here */

            builder.Entity<AppUser>(b =>
            {
                b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users"); //Sharing the same table "AbpUsers" with the IdentityUser

                b.ConfigureByConvention();
                b.ConfigureAbpUser();

                /* Configure mappings for your additional properties.
                 * Also see the MwpEfCoreEntityExtensionMappings class.
                 */
            });

            builder.ConfigureChat();

            //Share the same following tables: "Tenants", "TenantConnectionStrings" and "Editions"
            builder.ConfigureSaas();

            /* Configure your own tables/entities inside the ConfigureMwp method */

            builder.ConfigureMwp();
        }
    }
}