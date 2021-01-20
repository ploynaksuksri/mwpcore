using System;
using Volo.Abp.Application.Services;

namespace Mwp.Entities
{
    public interface IEntityAppService : ICrudAppService<EntityDto, Guid, GetEntitiesInput, EntityCreateDto, EntityUpdateDto>
    {
    }
}