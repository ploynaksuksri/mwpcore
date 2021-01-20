using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Financials.Reports;
using Mwp.Permissions;
using Mwp.Qbo.Dtos;
using Mwp.Qbo.Repositories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Qbo
{
    [Authorize(MwpPermissions.Qbo.Default)]
    public class QboAppService : MwpAppService, IQboAppService
    {
        protected readonly IQboReportApi _reportApi;
        protected readonly IRepository<QboTenant> _qboTenantRepository;
        protected readonly IQboTokenRepository _qboTokenRepository;
        protected readonly IQboAuthAppService _qboAuthAppService;

        public QboAppService(
            IRepository<QboTenant> qboTenantRepository,
            IQboTokenRepository qboTokenRepository,
            IQboReportApi reportApi,
            IQboAuthAppService qboAuthAppService)

        {
            _reportApi = reportApi;
            _qboAuthAppService = qboAuthAppService;
            _qboTenantRepository = qboTenantRepository;
            _qboTokenRepository = qboTokenRepository;
        }

        public async Task<PagedResultDto<QboTenantDto>> GetTenants()
        {
            var tenants = await _qboTenantRepository.GetListAsync();
            var tenantDtos = ObjectMapper.Map<List<QboTenant>, List<QboTenantDto>>(tenants);
            var tokens = await _qboTokenRepository.GetTokensByMwpUserId(CurrentUser.Id.Value);

            foreach (var tenant in tenantDtos)
            {
                tenant.IsConnected = tokens.Exists(e => e.QboTenantId == tenant.QboTenantId);
            }

            return new PagedResultDto<QboTenantDto>
            {
                TotalCount = tenants.Count,
                Items = tenantDtos
            };
        }

        public async Task<TrialBalanceReportDto> GetTrialBalance(string companyId)
        {
            var currentToken = await GetCurrentToken(companyId);
            var report = await _reportApi.GetTrialBalance(companyId, currentToken);
            return ObjectMapper.Map<TrialBalanceReport, TrialBalanceReportDto>(report);
        }

        async Task<QboToken> GetCurrentToken(string companyId)
        {
            var currentToken = await _qboAuthAppService.GetCurrentToken(companyId);
            return new QboToken
            {
                AccessToken = currentToken.AccessToken,
                MwpUserId = currentToken.MwpUserId
            };
        }
    }
}