using System;
using Volo.Abp.Application.Services;

namespace Mwp.Entities
{
    public interface IRelationshipTypeAppService : ICrudAppService<RelationshipTypeDto, Guid, GetRelationshipTypesInput, RelationshipTypeCreateDto, RelationshipTypeUpdateDto>
    {
    }
}