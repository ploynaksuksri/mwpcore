using Microsoft.Extensions.DependencyInjection;
using Mwp.AzureStorage.Form;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Azure;
using Volo.Abp.Modularity;

namespace Mwp.AzureStorage
{
    [DependsOn(
        typeof(MwpDomainModule),
        typeof(AbpBlobStoringAzureModule))]
    public class MwpAzureStorageModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            var configuration = context.Services.GetConfiguration();

            Configure<AbpBlobStoringOptions>(options =>
            {
                options.Containers.Configure<MwpFormBlobContainer>(container =>
                {
                    container.UseAzure(azure =>
                    {
                        azure.ConnectionString = configuration["Storage:ConnectionString"];
                        azure.CreateContainerIfNotExists = true;
                        azure.ContainerName = "test-form";
                    });
                });
            });
        }
    }
}