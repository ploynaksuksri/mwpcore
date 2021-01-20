using System;
using Volo.Abp.Application.Dtos;

namespace Mwp.Communications
{
    public class AddressDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}