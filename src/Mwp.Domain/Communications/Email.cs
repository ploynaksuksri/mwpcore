using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Communications
{
    public class Email : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual ICollection<CommunicationEmail> CommunicationEmail { get; set; }

        public Email()
        {

        }

        public Email(Guid id, Guid? tenantId, string name, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), EmailConsts.NameMaxLength, EmailConsts.NameMinLength);
            Name = name;

            IsActive = isActive;
        }
    }
}