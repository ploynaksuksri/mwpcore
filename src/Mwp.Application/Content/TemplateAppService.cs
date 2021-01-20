using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Content
{
    [Authorize(MwpPermissions.Templates.Default)]
    public class TemplateAppService : MwpAppService, ITemplateAppService
    {
        readonly IRepository<Template, Guid> _templateRepository;

        public TemplateAppService(IRepository<Template, Guid> templateRepository)
        {
            _templateRepository = templateRepository;
        }

        public virtual async Task<PagedResultDto<TemplateDto>> GetListAsync(GetTemplatesInput input)
        {
            var totalCount = await _templateRepository.GetCountAsync();
            var items = await _templateRepository.GetListAsync();

            return new PagedResultDto<TemplateDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Template>, List<TemplateDto>>(items)
            };
        }

        public virtual async Task<TemplateDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Template, TemplateDto>(await _templateRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Templates.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _templateRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Templates.Create)]
        public virtual async Task<TemplateDto> CreateAsync(TemplateCreateDto input)
        {
            var template = ObjectMapper.Map<TemplateCreateDto, Template>(input);
            template.TenantId = CurrentTenant.Id;
            template = await _templateRepository.InsertAsync(template, autoSave: true);
            return ObjectMapper.Map<Template, TemplateDto>(template);
        }

        [Authorize(MwpPermissions.Templates.Edit)]
        public virtual async Task<TemplateDto> UpdateAsync(Guid id, TemplateUpdateDto input)
        {
            var template = await _templateRepository.GetAsync(id);
            ObjectMapper.Map(input, template);
            template = await _templateRepository.UpdateAsync(template);
            return ObjectMapper.Map<Template, TemplateDto>(template);
        }
    }
}