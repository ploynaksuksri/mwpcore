using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Form
{
    public class Submission : FullAuditedEntity<Guid>, IMultiTenant
    {
        public Guid FormId { get; set; }

        public Form Form { get; set; }

        public string Data { get; set; }

        public Submission(Guid id, Guid formId) : base(id)
        {
            FormId = formId;
        }

        public Guid? TenantId { get; set; }
    }
}