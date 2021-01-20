using Intuit.Ipp.Core;
using Microsoft.Extensions.Configuration;
using Mwp.Qbo;
using Volo.Abp.DependencyInjection;

namespace Mwp.Services.ServiceContextFactory
{
    public class SandboxServiceContextFactory : BaseServiceContextFactory, IServiceContextFactory, IScopedDependency
    {
        private readonly string _qboBaseUrl;

        public SandboxServiceContextFactory(IConfiguration configuration)
        {
            _qboBaseUrl = configuration[QboConsts.BaseUrl];
        }

        public override ServiceContext Create(string realmId, string accessToken)
        {
            var serviceContext = base.Create(realmId, accessToken);
            serviceContext.IppConfiguration.BaseUrl.Qbo = _qboBaseUrl;
            return serviceContext;
        }
    }
}