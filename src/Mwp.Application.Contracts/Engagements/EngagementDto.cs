using System;
using Volo.Abp.Application.Dtos;

namespace Mwp.Engagements
{
    public class EngagementDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}