using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Form
{
    public class UserFormConfig : AuditedEntity<Guid>, IMultiTenant
    {
        public string FormBuilderConfig { get; set; }

        public UserFormConfig(Guid id) : base(id)
        {
        }

        public Guid? TenantId { get; set; }
    }
}