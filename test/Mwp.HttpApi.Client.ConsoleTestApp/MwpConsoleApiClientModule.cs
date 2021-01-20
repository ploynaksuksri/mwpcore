using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Mwp.HttpApi.Client.ConsoleTestApp
{
    [DependsOn(
        typeof(MwpHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
    )]
    public class MwpConsoleApiClientModule : AbpModule
    {
    }
}