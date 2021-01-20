using Mwp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Mwp
{
    [DependsOn(
        typeof(MwpEntityFrameworkCoreTestModule),
        typeof(MwpAzureResourceModule)
    )]
    public class MwpDomainTestModule : AbpModule
    {
    }
}