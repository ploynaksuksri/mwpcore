using Mwp.AzureStorage;
using Volo.Abp.Modularity;

namespace Mwp
{
    [DependsOn(
        typeof(MwpApplicationModule),
        typeof(MwpDomainTestModule),
        typeof(MwpAzureStorageModule)
    )]
    public class MwpApplicationTestModule : AbpModule
    {
    }
}