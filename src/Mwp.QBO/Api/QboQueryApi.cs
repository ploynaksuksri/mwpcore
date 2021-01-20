using System;
using System.Linq;
using Intuit.Ipp.Data;
using Mwp.Qbo;
using Mwp.Services;
using Volo.Abp.DependencyInjection;

namespace Mwp.Api
{
    public class QboQueryApi : ITransientDependency
    {
        private readonly QueryServiceFactory _queryServiceFactory;

        public QboQueryApi(QueryServiceFactory queryServiceFactory)
        {
            _queryServiceFactory = queryServiceFactory;
        }

        public QboTenant GetCompanyInfo(string companyId, QboToken qboToken)
        {
            var queryService = _queryServiceFactory.Create<CompanyInfo>(companyId, qboToken.AccessToken);
            var companyInfo = queryService.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();
            return new QboTenant
            {
                QboTenantId = companyId,
                Name = companyInfo.CompanyName,
                MwpTenantId = qboToken.MwpTenantId != Guid.Empty ? qboToken.MwpTenantId : (Guid?)null
            };
        }
    }
}