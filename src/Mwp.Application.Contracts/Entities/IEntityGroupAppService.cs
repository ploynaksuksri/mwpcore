using System;
using Volo.Abp.Application.Services;

namespace Mwp.Entities
{
    public interface IEntityGroupAppService : ICrudAppService<EntityGroupDto, Guid, GetEntityGroupsInput, EntityGroupCreateDto, EntityGroupUpdateDto>
    {
    }
}