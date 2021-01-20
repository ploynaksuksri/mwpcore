using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mwp.Entities;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Financials
{
    public class Ledger : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual Guid EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        public Ledger()
        {

        }

        public Ledger(Guid id, Guid? tenantId, string name, Guid entityId, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), LedgerConsts.NameMaxLength, LedgerConsts.NameMinLength);
            Name = name;

            EntityId = entityId;
            IsActive = isActive;
        }
    }
}