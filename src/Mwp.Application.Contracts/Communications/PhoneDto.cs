using System;
using Volo.Abp.Application.Dtos;

namespace Mwp.Communications
{
    public class PhoneDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}