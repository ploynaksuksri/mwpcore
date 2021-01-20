using System.Threading.Tasks;
using Mwp.Financials.Reports;
using Mwp.Qbo.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Mwp.Qbo
{
    public interface IQboAppService : IApplicationService
    {
        Task<PagedResultDto<QboTenantDto>> GetTenants();

        Task<TrialBalanceReportDto> GetTrialBalance(string companyId);
    }
}