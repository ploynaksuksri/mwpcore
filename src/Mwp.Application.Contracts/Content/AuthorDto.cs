using System;
using Volo.Abp.Application.Dtos;

namespace Mwp.Content
{
    public class AuthorDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}