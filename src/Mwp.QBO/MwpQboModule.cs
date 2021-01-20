using Volo.Abp.Modularity;

namespace Mwp
{
    [DependsOn(typeof(MwpDomainModule))]
    public class MwpQboModule : AbpModule
    {
    }
}