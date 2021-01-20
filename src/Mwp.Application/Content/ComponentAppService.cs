using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Content
{
    [Authorize(MwpPermissions.Components.Default)]
    public class ComponentAppService : MwpAppService, IComponentAppService
    {
        readonly IRepository<Component, Guid> _componentRepository;

        public ComponentAppService(IRepository<Component, Guid> componentRepository)
        {
            _componentRepository = componentRepository;
        }

        public virtual async Task<PagedResultDto<ComponentDto>> GetListAsync(GetComponentsInput input)
        {
            var totalCount = await _componentRepository.GetCountAsync();
            var items = await _componentRepository.GetListAsync();

            return new PagedResultDto<ComponentDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Component>, List<ComponentDto>>(items)
            };
        }

        public virtual async Task<ComponentDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Component, ComponentDto>(await _componentRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Components.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _componentRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Components.Create)]
        public virtual async Task<ComponentDto> CreateAsync(ComponentCreateDto input)
        {
            var component = ObjectMapper.Map<ComponentCreateDto, Component>(input);
            component.TenantId = CurrentTenant.Id;
            component = await _componentRepository.InsertAsync(component, autoSave: true);
            return ObjectMapper.Map<Component, ComponentDto>(component);
        }

        [Authorize(MwpPermissions.Components.Edit)]
        public virtual async Task<ComponentDto> UpdateAsync(Guid id, ComponentUpdateDto input)
        {
            var component = await _componentRepository.GetAsync(id);
            ObjectMapper.Map(input, component);
            component = await _componentRepository.UpdateAsync(component);
            return ObjectMapper.Map<Component, ComponentDto>(component);
        }
    }
}