using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Mwp.Tenants.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Tenants
{
    [Authorize(MwpPermissions.TenantTypes.Default)]
    public class TenantTypeAppService : MwpAppService, ITenantTypeAppService
    {
        readonly IRepository<TenantType, Guid> _tenantTypeRepository;

        public TenantTypeAppService(IRepository<TenantType, Guid> tenantTypeRepository)
        {
            _tenantTypeRepository = tenantTypeRepository;
        }

        public virtual async Task<PagedResultDto<TenantTypeDto>> GetListAsync(GetTenantTypesInput input)
        {
            var totalCount = await _tenantTypeRepository.GetCountAsync();
            var items = await _tenantTypeRepository.GetListAsync();

            return new PagedResultDto<TenantTypeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<TenantType>, List<TenantTypeDto>>(items)
            };
        }

        public virtual async Task<TenantTypeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<TenantType, TenantTypeDto>(await _tenantTypeRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.TenantTypes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _tenantTypeRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.TenantTypes.Create)]
        public virtual async Task<TenantTypeDto> CreateAsync(TenantTypeCreateDto input)
        {
            var tenantType = ObjectMapper.Map<TenantTypeCreateDto, TenantType>(input);
            tenantType.TenantId = CurrentTenant.Id;
            tenantType = await _tenantTypeRepository.InsertAsync(tenantType, autoSave: true);
            return ObjectMapper.Map<TenantType, TenantTypeDto>(tenantType);
        }

        [Authorize(MwpPermissions.TenantTypes.Edit)]
        public virtual async Task<TenantTypeDto> UpdateAsync(Guid id, TenantTypeUpdateDto input)
        {
            var tenantType = await _tenantTypeRepository.GetAsync(id);
            ObjectMapper.Map(input, tenantType);
            tenantType = await _tenantTypeRepository.UpdateAsync(tenantType);
            return ObjectMapper.Map<TenantType, TenantTypeDto>(tenantType);
        }
    }
}