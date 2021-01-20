using System;
using System.Threading.Tasks;
using Mwp.Financials.Reports;
using Mwp.Xero.Reports;
using Volo.Abp.DependencyInjection;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Client;

namespace Mwp.Xero.Api
{
    public class XeroAccountingApi : IXeroAccountingApi, ITransientDependency
    {
        public async Task<TrialBalanceReport> GetTrialBalance(Guid xeroTenantId, XeroToken xeroToken)
        {
            try
            {
                var response = await new AccountingApi()
                    .GetReportTrialBalanceAsync(xeroToken.AccessToken, xeroTenantId.ToString());

                return TrialBalanceReportMapper.Map(response.Reports[0]);
            }
            catch (ApiException exception)
            {
                throw XeroExceptionHandler.ParseException(exception);
            }
        }
    }
}