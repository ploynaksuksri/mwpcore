using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Communications
{
    public class Address : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual ICollection<CommunicationAddress> CommunicationAddress { get; set; }

        public Address()
        {

        }

        public Address(Guid id, Guid? tenantId, string name, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), AddressConsts.NameMaxLength, AddressConsts.NameMinLength);
            Name = name;

            IsActive = isActive;
        }
    }
}