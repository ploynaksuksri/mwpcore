using System;
using System.Threading.Tasks;
using Mwp.Financials.Reports;
using Mwp.Xero.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Mwp.Xero
{
    public interface IXeroAppService : IApplicationService
    {
        Task<PagedResultDto<XeroTenantDto>> GetTenants();

        Task<TrialBalanceReportDto> GetTrialBalance(Guid xeroTenantId);
    }
}