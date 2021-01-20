using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Entities
{
    [Authorize(MwpPermissions.RelationshipTypes.Default)]
    public class RelationshipTypeAppService : MwpAppService, IRelationshipTypeAppService
    {
        readonly IRepository<RelationshipType, Guid> _relationshipTypeRepository;

        public RelationshipTypeAppService(IRepository<RelationshipType, Guid> relationshipTypeRepository)
        {
            _relationshipTypeRepository = relationshipTypeRepository;
        }

        public virtual async Task<PagedResultDto<RelationshipTypeDto>> GetListAsync(GetRelationshipTypesInput input)
        {
            var totalCount = await _relationshipTypeRepository.GetCountAsync();
            var items = await _relationshipTypeRepository.GetListAsync();

            return new PagedResultDto<RelationshipTypeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<RelationshipType>, List<RelationshipTypeDto>>(items)
            };
        }

        public virtual async Task<RelationshipTypeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<RelationshipType, RelationshipTypeDto>(await _relationshipTypeRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.RelationshipTypes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _relationshipTypeRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.RelationshipTypes.Create)]
        public virtual async Task<RelationshipTypeDto> CreateAsync(RelationshipTypeCreateDto input)
        {
            var relationshipType = ObjectMapper.Map<RelationshipTypeCreateDto, RelationshipType>(input);
            relationshipType.TenantId = CurrentTenant.Id;
            relationshipType = await _relationshipTypeRepository.InsertAsync(relationshipType, autoSave: true);
            return ObjectMapper.Map<RelationshipType, RelationshipTypeDto>(relationshipType);
        }

        [Authorize(MwpPermissions.RelationshipTypes.Edit)]
        public virtual async Task<RelationshipTypeDto> UpdateAsync(Guid id, RelationshipTypeUpdateDto input)
        {
            var relationshipType = await _relationshipTypeRepository.GetAsync(id);
            ObjectMapper.Map(input, relationshipType);
            relationshipType = await _relationshipTypeRepository.UpdateAsync(relationshipType);
            return ObjectMapper.Map<RelationshipType, RelationshipTypeDto>(relationshipType);
        }
    }
}