using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Mwp.Entities;
using Volo.Abp;

namespace Mwp.Communications
{
    public class Communication : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual ICollection<EntityCommunication> EntityCommunication { get; set; }

        public virtual ICollection<CommunicationAddress> CommunicationAddress { get; set; }

        public virtual ICollection<CommunicationEmail> CommunicationEmail { get; set; }

        public virtual ICollection<CommunicationPhone> CommunicationPhone { get; set; }

        public virtual ICollection<CommunicationWebsite> CommunicationWebsite { get; set; }

        public Communication()
        {

        }

        public Communication(Guid id, Guid? tenantId, string name, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), CommunicationConsts.NameMaxLength, CommunicationConsts.NameMinLength);
            Name = name;

            IsActive = isActive;
        }
    }
}