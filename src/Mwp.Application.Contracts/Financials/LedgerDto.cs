using System;
using Volo.Abp.Application.Dtos;

namespace Mwp.Financials
{
    public class LedgerDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}