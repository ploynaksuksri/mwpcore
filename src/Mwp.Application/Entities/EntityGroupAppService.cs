using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Entities
{
    [Authorize(MwpPermissions.EntityGroups.Default)]
    public class EntityGroupAppService : MwpAppService, IEntityGroupAppService
    {
        readonly IRepository<EntityGroup, Guid> _entityGroupRepository;

        public EntityGroupAppService(IRepository<EntityGroup, Guid> entityGroupRepository)
        {
            _entityGroupRepository = entityGroupRepository;
        }

        public virtual async Task<PagedResultDto<EntityGroupDto>> GetListAsync(GetEntityGroupsInput input)
        {
            var totalCount = await _entityGroupRepository.GetCountAsync();
            var items = await _entityGroupRepository.GetListAsync();

            return new PagedResultDto<EntityGroupDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<EntityGroup>, List<EntityGroupDto>>(items)
            };
        }

        public virtual async Task<EntityGroupDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<EntityGroup, EntityGroupDto>(await _entityGroupRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.EntityGroups.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _entityGroupRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.EntityGroups.Create)]
        public virtual async Task<EntityGroupDto> CreateAsync(EntityGroupCreateDto input)
        {
            var entityGroup = ObjectMapper.Map<EntityGroupCreateDto, EntityGroup>(input);
            entityGroup.TenantId = CurrentTenant.Id;
            entityGroup = await _entityGroupRepository.InsertAsync(entityGroup, autoSave: true);
            return ObjectMapper.Map<EntityGroup, EntityGroupDto>(entityGroup);
        }

        [Authorize(MwpPermissions.EntityGroups.Edit)]
        public virtual async Task<EntityGroupDto> UpdateAsync(Guid id, EntityGroupUpdateDto input)
        {
            var entityGroup = await _entityGroupRepository.GetAsync(id);
            ObjectMapper.Map(input, entityGroup);
            entityGroup = await _entityGroupRepository.UpdateAsync(entityGroup);
            return ObjectMapper.Map<EntityGroup, EntityGroupDto>(entityGroup);
        }
    }
}