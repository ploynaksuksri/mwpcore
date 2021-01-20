using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Financials.Reports;
using Mwp.Permissions;
using Mwp.Xero.Dtos;
using Mwp.Xero.Repositories;
using Volo.Abp.Application.Dtos;

namespace Mwp.Xero
{
    [Authorize(MwpPermissions.Xero.Default)]
    public class XeroAppService : MwpAppService, IXeroAppService
    {
        protected IXeroTenantRepository XeroTenantRepository;
        protected IXeroConnectionRepository XeroConnectionRepository;
        protected IXeroAccountingApi XeroAccountingApi;
        protected IXeroAuthAppService XeroAuthAppService;

        public XeroAppService(
            IXeroTenantRepository xeroTenantRepository,
            IXeroConnectionRepository xeroConnectionRepository,
            IXeroAccountingApi xeroAccountingApi,
            IXeroAuthAppService xeroAuthAppService)
        {
            XeroTenantRepository = xeroTenantRepository;
            XeroConnectionRepository = xeroConnectionRepository;
            XeroAccountingApi = xeroAccountingApi;
            XeroAuthAppService = xeroAuthAppService;
        }

        public async Task<PagedResultDto<XeroTenantDto>> GetTenants()
        {
            var xeroTenants = await XeroTenantRepository.GetByMwpTenantId(CurrentTenant.Id);

            var connections = await XeroConnectionRepository.GetByMwpUserId(CurrentUser.Id ?? Guid.Empty);

            var xeroTenantDtos = ObjectMapper.Map<List<XeroTenant>, List<XeroTenantDto>>(xeroTenants);

            foreach (var tenant in xeroTenantDtos)
            {
                tenant.IsConnected = connections.Exists(e => e.XeroTenantId == tenant.XeroTenantId);
            }

            return new PagedResultDto<XeroTenantDto>(
                xeroTenants.Count,
                xeroTenantDtos
            );
        }

        public async Task<TrialBalanceReportDto> GetTrialBalance(Guid xeroTenantId)
        {
            var currentToken = await GetCurrentToken(xeroTenantId);
            var report = await XeroAccountingApi.GetTrialBalance(xeroTenantId, currentToken);
            return ObjectMapper.Map<TrialBalanceReport, TrialBalanceReportDto>(report);
        }

        async Task<XeroToken> GetCurrentToken(Guid xeroTenantId)
        {
            var xeroTokenDto = await XeroAuthAppService.GetCurrentToken(xeroTenantId);
            return new XeroToken
            {
                AccessToken = xeroTokenDto.AccessToken,
                MwpUserId = xeroTokenDto.MwpUserId
            };
        }
    }
}