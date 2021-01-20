using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Communications
{
    [Authorize(MwpPermissions.Websites.Default)]
    public class WebsiteAppService : MwpAppService, IWebsiteAppService
    {
        readonly IRepository<Website, Guid> _websiteRepository;

        public WebsiteAppService(IRepository<Website, Guid> websiteRepository)
        {
            _websiteRepository = websiteRepository;
        }

        public virtual async Task<PagedResultDto<WebsiteDto>> GetListAsync(GetWebsitesInput input)
        {
            var totalCount = await _websiteRepository.GetCountAsync();
            var items = await _websiteRepository.GetListAsync();

            return new PagedResultDto<WebsiteDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Website>, List<WebsiteDto>>(items)
            };
        }

        public virtual async Task<WebsiteDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Website, WebsiteDto>(await _websiteRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Websites.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _websiteRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Websites.Create)]
        public virtual async Task<WebsiteDto> CreateAsync(WebsiteCreateDto input)
        {
            var website = ObjectMapper.Map<WebsiteCreateDto, Website>(input);
            website.TenantId = CurrentTenant.Id;
            website = await _websiteRepository.InsertAsync(website, autoSave: true);
            return ObjectMapper.Map<Website, WebsiteDto>(website);
        }

        [Authorize(MwpPermissions.Websites.Edit)]
        public virtual async Task<WebsiteDto> UpdateAsync(Guid id, WebsiteUpdateDto input)
        {
            var website = await _websiteRepository.GetAsync(id);
            ObjectMapper.Map(input, website);
            website = await _websiteRepository.UpdateAsync(website);
            return ObjectMapper.Map<Website, WebsiteDto>(website);
        }
    }
}