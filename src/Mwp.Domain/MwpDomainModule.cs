using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mwp.DataSeeder;
using Mwp.MultiTenancy;
using Mwp.Settings;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.Commercial.SuiteTemplates;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.IdentityServer;
using Volo.Abp.Reflection;
using Volo.Abp.Security;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Sms;
using Volo.Abp.Sms.Twilio;
using Volo.Abp.TextTemplateManagement;
using Volo.Chat;
using Volo.Saas;

namespace Mwp
{
    [DependsOn(
        typeof(MwpDomainSharedModule),
        typeof(AbpAuditLoggingDomainModule),
        typeof(AbpBackgroundJobsDomainModule),
        typeof(AbpFeatureManagementDomainModule),
        typeof(AbpIdentityDomainModule),
        typeof(AbpPermissionManagementDomainIdentityModule),
        typeof(AbpIdentityServerDomainModule),
        typeof(AbpPermissionManagementDomainIdentityServerModule),
        typeof(AbpSettingManagementDomainModule),
        typeof(SaasDomainModule),
        typeof(TextTemplateManagementDomainModule),
        typeof(LeptonThemeManagementDomainModule),
        typeof(LanguageManagementDomainModule),
        typeof(VoloAbpCommercialSuiteTemplatesModule),
        typeof(AbpEmailingModule),
        typeof(BlobStoringDatabaseDomainModule),
        typeof(ChatDomainModule),
        typeof(AbpSecurityModule),
        typeof(AbpTwilioSmsModule)
    )]
    public class MwpDomainModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.OnExposing(onServiceExposingContext =>
            {
                //Register types for IMwpDataSeeder<T> if implements
                onServiceExposingContext.ExposedTypes.AddRange(
                    ReflectionHelper.GetImplementedGenericTypes(
                        onServiceExposingContext.ImplementationType,
                        typeof(IMwpDataSeeder<,>)
                    )
                );

                onServiceExposingContext.ExposedTypes.AddRange(
                    ReflectionHelper.GetImplementedGenericTypes(
                        onServiceExposingContext.ImplementationType,
                        typeof(IMwpDataSeeder<,,>)
                    )
                );
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMultiTenancyOptions>(options =>
            {
                options.IsEnabled = MultiTenancyConsts.IsEnabled;
            });

            Configure<AbpSettingOptions>(options =>
            {
                options.ValueProviders.InsertAfter(typeof(ConfigurationSettingValueProvider), typeof(MwpConfigurationWithUnderscoreSettingValueProvider));
                options.ValueProviders.InsertAfter(typeof(MwpConfigurationWithUnderscoreSettingValueProvider), typeof(MwpConfigurationWithDashSettingValueProvider));
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("en", "en", "English", "gb"));
                options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe", "tr"));
                options.Languages.Add(new LanguageInfo("sl", "sl", "Slovenščina", "si"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文", "cn"));
                options.Languages.Add(new LanguageInfo("nl", "nl", "Nederlands"));
                options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
                options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
                options.Languages.Add(new LanguageInfo("sl", "sl", "Slovenščina"));
                options.Languages.Add(new LanguageInfo("th", "th", "ไทย"));
                options.Languages.Add(new LanguageInfo("vi", "vi", "Tiếng Việt"));
                options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
                options.Languages.Add(new LanguageInfo("de", "de", "Deutsche"));
            });

#if DEBUG
            context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
            context.Services.Replace(ServiceDescriptor.Singleton<ISmsSender, NullSmsSender>());
#endif
        }
    }
}