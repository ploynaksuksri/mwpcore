using Volo.Abp.Modularity;

namespace Mwp.Xero
{
    [DependsOn(typeof(MwpDomainModule))]
    public class MwpXeroModule : AbpModule
    {
    }
}