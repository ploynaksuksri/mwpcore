using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Entities
{
    [Authorize(MwpPermissions.EntityTypes.Default)]
    public class EntityTypeAppService : MwpAppService, IEntityTypeAppService
    {
        readonly IRepository<EntityType, Guid> _entityTypeRepository;

        public EntityTypeAppService(IRepository<EntityType, Guid> entityTypeRepository)
        {
            _entityTypeRepository = entityTypeRepository;
        }

        public virtual async Task<PagedResultDto<EntityTypeDto>> GetListAsync(GetEntityTypesInput input)
        {
            var totalCount = await _entityTypeRepository.GetCountAsync();
            var items = await _entityTypeRepository.GetListAsync();

            return new PagedResultDto<EntityTypeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<EntityType>, List<EntityTypeDto>>(items)
            };
        }

        public virtual async Task<EntityTypeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<EntityType, EntityTypeDto>(await _entityTypeRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.EntityTypes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _entityTypeRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.EntityTypes.Create)]
        public virtual async Task<EntityTypeDto> CreateAsync(EntityTypeCreateDto input)
        {
            var entityType = ObjectMapper.Map<EntityTypeCreateDto, EntityType>(input);
            entityType.TenantId = CurrentTenant.Id;
            entityType = await _entityTypeRepository.InsertAsync(entityType, autoSave: true);
            return ObjectMapper.Map<EntityType, EntityTypeDto>(entityType);
        }

        [Authorize(MwpPermissions.EntityTypes.Edit)]
        public virtual async Task<EntityTypeDto> UpdateAsync(Guid id, EntityTypeUpdateDto input)
        {
            var entityType = await _entityTypeRepository.GetAsync(id);
            ObjectMapper.Map(input, entityType);
            entityType = await _entityTypeRepository.UpdateAsync(entityType);
            return ObjectMapper.Map<EntityType, EntityTypeDto>(entityType);
        }
    }
}