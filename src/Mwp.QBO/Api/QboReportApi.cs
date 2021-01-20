using System.Threading.Tasks;
using Mwp.Financials.Reports;
using Mwp.Qbo;
using Mwp.Reports;
using Mwp.Services;
using Volo.Abp.DependencyInjection;

namespace Mwp.Api
{
    public class QboReportApi : IQboReportApi, ITransientDependency
    {
        readonly ReportServiceFactory _reportServiceFactory;

        public QboReportApi(ReportServiceFactory reportServiceFactory)
        {
            _reportServiceFactory = reportServiceFactory;
        }

        public Task<TrialBalanceReport> GetTrialBalance(string companyId, QboToken qboToken)
        {
            var service = _reportServiceFactory.Create(companyId, qboToken.AccessToken);
            var report = service.ExecuteReport(QboConsts.TrialBalanceReport);
            return Task.FromResult(TrialBalanceReportMapper.Map(report));
        }
    }
}