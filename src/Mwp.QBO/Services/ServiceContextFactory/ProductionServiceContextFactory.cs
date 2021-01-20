using Volo.Abp.DependencyInjection;

namespace Mwp.Services.ServiceContextFactory
{
    public class ProductionServiceContextFactory : BaseServiceContextFactory, IServiceContextFactory, IScopedDependency
    {
    }
}