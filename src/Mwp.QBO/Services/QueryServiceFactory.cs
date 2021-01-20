using System;
using Intuit.Ipp.QueryFilter;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace Mwp.Services
{
    public class QueryServiceFactory : BaseQboServiceFactory, ITransientDependency
    {
        public QueryServiceFactory(
            IConfiguration configuration,
            IServiceProvider serviceProvider)
            : base(configuration, serviceProvider)
        {
        }

        public QueryService<TEntity> Create<TEntity>(string realmId, string accessToken)
        {
            var serviceContext = _serviceContextFactory.Create(realmId, accessToken);
            var queryService = new QueryService<TEntity>(serviceContext);
            return queryService;
        }
    }
}