using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Communications
{
    public class Phone : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual ICollection<CommunicationPhone> CommunicationPhone { get; set; }

        public Phone()
        {

        }

        public Phone(Guid id, Guid? tenantId, string name, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), PhoneConsts.NameMaxLength, PhoneConsts.NameMinLength);
            Name = name;

            IsActive = isActive;
        }
    }
}