using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Mwp.Permissions;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Entities
{
    [Authorize(MwpPermissions.Entities.Default)]
    public class EntityAppService : MwpAppService, IEntityAppService
    {
        readonly IRepository<Entity, Guid> _entityRepository;

        public EntityAppService(IRepository<Entity, Guid> entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public virtual async Task<PagedResultDto<EntityDto>> GetListAsync(GetEntitiesInput input)
        {
            var totalCount = await _entityRepository.GetCountAsync();
            var items = await _entityRepository.GetListAsync();

            return new PagedResultDto<EntityDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Entity>, List<EntityDto>>(items)
            };
        }

        public virtual async Task<EntityDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Entity, EntityDto>(await _entityRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Entities.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _entityRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Entities.Create)]
        public virtual async Task<EntityDto> CreateAsync(EntityCreateDto input)
        {
            var entity = ObjectMapper.Map<EntityCreateDto, Entity>(input);
            entity.TenantId = CurrentTenant.Id;
            entity = await _entityRepository.InsertAsync(entity, autoSave: true);
            return ObjectMapper.Map<Entity, EntityDto>(entity);
        }

        [Authorize(MwpPermissions.Entities.Edit)]
        public virtual async Task<EntityDto> UpdateAsync(Guid id, EntityUpdateDto input)
        {
            var entity = await _entityRepository.GetAsync(id);
            ObjectMapper.Map(input, entity);
            entity = await _entityRepository.UpdateAsync(entity);
            return ObjectMapper.Map<Entity, EntityDto>(entity);
        }
    }
}