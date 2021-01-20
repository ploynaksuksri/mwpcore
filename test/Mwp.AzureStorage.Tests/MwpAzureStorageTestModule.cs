using Mwp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Mwp.AzureStorage
{
    [DependsOn(
        typeof(MwpEntityFrameworkCoreTestModule),
        typeof(MwpAzureStorageModule))]
    public class MwpAzureStorageTestModule : AbpModule
    {
    }
}