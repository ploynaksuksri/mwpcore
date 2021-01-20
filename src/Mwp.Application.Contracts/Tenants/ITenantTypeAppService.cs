using System;
using Mwp.Tenants.Dtos;
using Volo.Abp.Application.Services;

namespace Mwp.Tenants
{
    public interface ITenantTypeAppService : ICrudAppService<TenantTypeDto, Guid, GetTenantTypesInput, TenantTypeCreateDto, TenantTypeUpdateDto>
    {
    }
}