using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Engagements
{
    public class Document : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual Guid WorkpaperId { get; set; }

        public virtual Workpaper Workpaper { get; set; }

        public Document()
        {

        }

        public Document(Guid id, Guid? tenantId, string name, Guid workpaperId, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), DocumentConsts.NameMaxLength, DocumentConsts.NameMinLength);
            Name = name;

            WorkpaperId = workpaperId;
            IsActive = isActive;
        }
    }
}