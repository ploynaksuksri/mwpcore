using System;
using Intuit.Ipp.ReportService;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace Mwp.Services
{
    public class ReportServiceFactory : BaseQboServiceFactory, ITransientDependency
    {
        public ReportServiceFactory(
            IConfiguration configuration,
            IServiceProvider serviceProvider)
            : base(configuration, serviceProvider)
        {
        }

        public ReportService Create(string realmId, string accessToken)
        {
            var serviceContext = _serviceContextFactory.Create(realmId, accessToken);
            var queryService = new ReportService(serviceContext);
            return queryService;
        }
    }
}