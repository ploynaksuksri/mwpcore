using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Mwp.AzureStorage;
using Mwp.Conventions;
using Mwp.EntityFrameworkCore;
using Mwp.Extensions;
using Mwp.HealthChecks;
using Mwp.MultiTenancy;
using Mwp.Xero;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerUI;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Chat;

namespace Mwp
{
    [DependsOn(
        typeof(MwpHttpApiModule),
        typeof(AbpAutofacModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
        typeof(AbpIdentityAspNetCoreModule),
        typeof(MwpApplicationModule),
        typeof(MwpEntityFrameworkCoreDbMigrationsModule),
        typeof(MwpAzureStorageModule),
        typeof(MwpXeroModule),
        typeof(MwpQboModule),
        typeof(MwpAzureResourceModule),
        typeof(ChatSignalRModule)
    )]
    public class MwpHttpApiHostModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            ConfigureUrls(configuration);
            ConfigureConventionalControllers();
            ConfigureAuthentication(context, configuration);
            ConfigureSwagger(context);
            ConfigureCache(configuration);
            ConfigureVirtualFileSystem(context);
            ConfigureRedis(context, configuration, hostingEnvironment);
            ConfigureCors(context, configuration);
            ConfigureExternalProviders(context);
            ConfigureAppInsight(context, hostingEnvironment);
            ConfigureHealthChecks(context, configuration);
            ConfigureBackgroudJob(false);
            ConfigureAntiForgery();
        }

        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>(options =>
            {
                options.Applications["Angular"].RootUrl = configuration[MwpConsts.ClientUrl];
                options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
                options.Applications["Angular"].Urls[AccountUrlNames.EmailConfirmation] = "account/email-confirmation";
                options.Applications[MwpConsts.AppName].Urls[MwpConsts.SelfUrl] = configuration[MwpConsts.SelfUrl];
                options.Applications[MwpConsts.AppName].Urls[MwpConsts.ClientUrl] = configuration[MwpConsts.ClientUrl];
            });
        }


        private void ConfigureCache(IConfiguration configuration)
        {
            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.KeyPrefix = "Mwp:";
                options.GlobalCacheEntryOptions.SlidingExpiration = TimeSpan.FromHours(1);
            });
        }

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<MwpDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Mwp.Domain.Shared", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<MwpDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Mwp.Domain", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<MwpApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Mwp.Application.Contracts", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<MwpApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Mwp.Application", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<MwpHttpApiModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Mwp.HttpApi", Path.DirectorySeparatorChar)));
                });
            }
        }

        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(MwpApplicationModule).Assembly);
            });

            Configure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new MwpServiceConventionWrapper());
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.Audience = "Mwp";
                });
        }

        private static void ConfigureSwagger(ServiceConfigurationContext context)
        {
            context.Services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = $"{MwpConsts.CompanyName} API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme());
                });
        }

        private void ConfigureRedis(
            ServiceConfigurationContext context,
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment)
        {
            if (bool.Parse(configuration["Redis:IsEnabled"]))
            {
                var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
                context.Services
                    .AddDataProtection()
                    .PersistKeysToStackExchangeRedis(redis, "Mwp-Protection-Keys");
            }
        }

        private void ConfigureAppInsight(ServiceConfigurationContext context, IWebHostEnvironment hostingEnvironment)
        {
            if (!hostingEnvironment.IsDevelopment())
            {
                context.Services.AddApplicationInsightsTelemetry();
            }
        }

        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .WithExposedHeaders(configuration["App:ExposedHeaders"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .ToArray()
                        )
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        private void ConfigureExternalProviders(ServiceConfigurationContext context)
        {
            context.Services
                .AddDynamicExternalLoginProviderOptions<GoogleOptions>(
                    GoogleDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.WithProperty(x => x.ClientId);
                        options.WithProperty(x => x.ClientSecret, isSecret: true);
                    }
                )
                .AddDynamicExternalLoginProviderOptions<MicrosoftAccountOptions>(
                    MicrosoftAccountDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.WithProperty(x => x.ClientId);
                        options.WithProperty(x => x.ClientSecret, isSecret: true);
                    }
                )
                .AddDynamicExternalLoginProviderOptions<TwitterOptions>(
                    TwitterDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.WithProperty(x => x.ConsumerKey);
                        options.WithProperty(x => x.ConsumerSecret, isSecret: true);
                    }
                );
        }

        private void ConfigureHealthChecks(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddHealthChecks()
                .AddSqlServer(configuration[MwpConsts.ConnectionStringsDefault])
                .AddCheck("appinfo", () => HealthCheckResult.Healthy("Application Information",
                    new Dictionary<string, object>
                    {
                        { "version", GetType().Assembly.GetName().Version?.ToString(3) }
                    }));
        }

        private void ConfigureBackgroudJob(bool isEnabled)
        {
            Configure<AbpBackgroundJobOptions>(options => { options.IsJobExecutionEnabled = isEnabled; });
        }

        private void ConfigureAntiForgery()
        {
            Configure<AbpAntiForgeryOptions>(options =>
            {
                options.TokenCookie.Name = "XSRF-TOKEN";
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            /*if (!env.IsDevelopment())
            {
                app.UseErrorPage();
            }*/

            app.UseCors(DefaultCorsPolicyName);

            app.UseVirtualFiles();
            app.UseRouting();
            app.UseAuthentication();

            if (MultiTenancyConsts.IsEnabled)
            {
                app.UseMultiTenancy();
            }

            app.UseSignalRHubAuthorization(MwpConsts.ChatHubPathPrefix);
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{MwpConsts.CompanyName} API");
                options.DocumentTitle = $"{MwpConsts.CompanyName}";
                options.IndexStream = () => GetType().Assembly.GetManifestResourceStream("Mwp.wwwroot.swagger.ui.index.html");
                var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
                options.ConfigObject.AdditionalItems["Authority"] = configuration["AuthServer:Authority"];
                options.ConfigObject.AdditionalItems["SwaggerClientId"] = configuration["AuthServer:SwaggerClientId"];
                options.ConfigObject.AdditionalItems["SwaggerClientSecret"] = configuration["AuthServer:SwaggerClientSecret"];
                options.ConfigObject.AdditionalItems["SwaggerScope"] = configuration["AuthServer:SwaggerScope"];
                app.UseMiddleware<SwaggerUIMiddleware>((object)options);
            });
            app.UseAuditing();
            app.UseConfiguredEndpoints();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    AllowCachingResponses = false,
                    ResponseWriter = HealthCheckResponseWriter.WriteResponse
                });
            });
        }
    }
}