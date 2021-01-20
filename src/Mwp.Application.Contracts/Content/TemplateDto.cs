using System;
using Volo.Abp.Application.Dtos;

namespace Mwp.Content
{
    public class TemplateDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}