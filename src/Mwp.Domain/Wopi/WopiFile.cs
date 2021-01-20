using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Wopi
{
    public class WopiFile : AuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid? SubmissionId { get; set; }

        public Guid? FormId { get; set; }

        public string LockValue { get; set; }

        public DateTime? LockExpires { get; set; }

        public List<WopiFileHistory> Histories { get; set; }
        public Guid? CheckoutBy { get; set; }
        public DateTime? CheckoutTimestamp { get; set; }

        protected WopiFile()
        {
            Histories = new List<WopiFileHistory>();
        }

        public WopiFile(Guid id) : base(id)
        {
            Histories = new List<WopiFileHistory>();
        }
    }
}