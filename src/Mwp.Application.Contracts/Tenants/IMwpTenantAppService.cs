using System;
using System.Threading.Tasks;
using Mwp.Tenants.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Saas.Host;
using Volo.Saas.Host.Dtos;

namespace Mwp.Tenants
{
    public interface IMwpTenantAppService : ITenantAppService
    {
        new Task<MwpSaasTenantDto> GetAsync(Guid id);

        new Task<PagedResultDto<MwpSaasTenantDto>> GetListAsync(GetTenantsInput input);

        Task<SaasTenantDto> CreateAsync(MwpSaasTenantCreateDto input);
    }
}