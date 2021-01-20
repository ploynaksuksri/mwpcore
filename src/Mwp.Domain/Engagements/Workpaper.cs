using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Engagements
{
    public class Workpaper : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual Guid FolderId { get; set; }

        public virtual Folder Folder { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        public virtual ICollection<WorkpaperComponent> WorkpaperComponent { get; set; }

        public Workpaper()
        {

        }

        public Workpaper(Guid id, Guid? tenantId, string name, Guid folderId, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), WorkpaperConsts.NameMaxLength, WorkpaperConsts.NameMinLength);
            Name = name;

            FolderId = folderId;
            IsActive = isActive;
        }
    }
}