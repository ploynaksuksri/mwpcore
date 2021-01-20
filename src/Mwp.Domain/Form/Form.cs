using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Form
{
    public class Form : FullAuditedEntity<Guid>, IMultiTenant
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public Guid? ParentId { get; set; }
        public string HierarchicalPath { get; set; }
        public Guid? TenantId { get; set; }
        public string ParentDetail { get; set; }
        public Guid CurrentVersion { get; set; }

        public Form(Guid id) : base(id)
        {
        }
    }
}