using System.Threading.Tasks;
using Mwp.Financials.Reports;

namespace Mwp.Qbo
{
    public interface IQboReportApi
    {
        Task<TrialBalanceReport> GetTrialBalance(string companyId, QboToken qboToken);
    }
}