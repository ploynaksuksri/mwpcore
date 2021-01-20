using System;
using System.Threading.Tasks;
using Mwp.Financials.Reports;

namespace Mwp.Xero
{
    public interface IXeroAccountingApi
    {
        Task<TrialBalanceReport> GetTrialBalance(Guid xeroTenantId, XeroToken xeroToken);
    }
}