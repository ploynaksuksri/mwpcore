using System;
using Volo.Abp.Application.Dtos;

namespace Mwp.Entities
{
    public class RelationshipTypeDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}