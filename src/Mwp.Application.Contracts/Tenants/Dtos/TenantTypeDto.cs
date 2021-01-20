using System;
using Volo.Abp.Application.Dtos;

namespace Mwp.Tenants.Dtos
{
    public class TenantTypeDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}