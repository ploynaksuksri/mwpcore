using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mwp.Tenants;
using Mwp.Tenants.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Saas.Host;
using Volo.Saas.Host.Dtos;

namespace Mwp.Controllers
{
    [Controller]
    [RemoteService(Name = SaasHostRemoteServiceConsts.RemoteServiceName)]
    [Area("saas")]
    [ControllerName("Tenant")]
    [Route("/api/saas/tenants")]
    public class MwpTenantController : MwpController
    {
        private IMwpTenantAppService Service { get; }

        public MwpTenantController(IMwpTenantAppService service)
        {
            Service = service;
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<MwpSaasTenantDto> GetAsync(Guid id)
        {
            return Service.GetAsync(id);
        }

        [HttpGet]
        public virtual Task<PagedResultDto<MwpSaasTenantDto>> GetListAsync(GetTenantsInput input)
        {
            return Service.GetListAsync(input);
        }

        [HttpPost]
        public virtual Task<SaasTenantDto> CreateAsync(MwpSaasTenantCreateDto input)
        {
            ValidateModel();
            return Service.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<SaasTenantDto> UpdateAsync(Guid id, SaasTenantUpdateDto input)
        {
            return Service.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return Service.DeleteAsync(id);
        }

        [HttpGet]
        [Route("{id}/default-connection-string")]
        public virtual Task<string> GetDefaultConnectionStringAsync(Guid id)
        {
            return Service.GetDefaultConnectionStringAsync(id);
        }

        [HttpPut]
        [Route("{id}/default-connection-string")]
        public virtual Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
        {
            return Service.UpdateDefaultConnectionStringAsync(id, defaultConnectionString);
        }

        [HttpDelete]
        [Route("{id}/default-connection-string")]
        public virtual Task DeleteDefaultConnectionStringAsync(Guid id)
        {
            return Service.DeleteDefaultConnectionStringAsync(id);
        }
    }
}
