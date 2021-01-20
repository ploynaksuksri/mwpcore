using Mwp.File.BackgroundWorkers;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AuditLogging;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TextTemplateManagement;
using Volo.Chat;
using Volo.Saas.Host;

namespace Mwp
{
    [DependsOn(
        typeof(MwpDomainModule),
        typeof(MwpApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(SaasHostApplicationModule),
        typeof(AbpAuditLoggingApplicationModule),
        typeof(AbpIdentityServerApplicationModule),
        typeof(AbpAccountPublicApplicationModule),
        typeof(AbpAccountAdminApplicationModule),
        typeof(LanguageManagementApplicationModule),
        typeof(LeptonThemeManagementApplicationModule),
        typeof(TextTemplateManagementApplicationModule),
        typeof(AbpBackgroundWorkersModule),
        typeof(ChatApplicationModule))]
    public class MwpApplicationModule : AbpModule
    {
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.AddBackgroundWorker<ClearUnusedFileWorker>();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<MwpApplicationModule>();
            });
        }
    }
}