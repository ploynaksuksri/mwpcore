using System;
using Volo.Abp.Application.Services;

namespace Mwp.Entities
{
    public interface IEntityTypeAppService : ICrudAppService<EntityTypeDto, Guid, GetEntityTypesInput, EntityTypeCreateDto, EntityTypeUpdateDto>
    {
    }
}