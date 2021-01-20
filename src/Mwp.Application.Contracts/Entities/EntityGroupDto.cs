using System;
using Volo.Abp.Application.Dtos;

namespace Mwp.Entities
{
    public class EntityGroupDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}