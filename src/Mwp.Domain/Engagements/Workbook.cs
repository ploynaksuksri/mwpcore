using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mwp.Content;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Engagements
{
    public class Workbook : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual Guid EngagementId { get; set; }

        public virtual Engagement Engagement { get; set; }

        public virtual ICollection<Folder> Folders { get; set; }

        // 1 to 0..1 relationship
        public virtual ICollection<Title> Title { get; set; }

        public Workbook()
        {

        }

        public Workbook(Guid id, Guid? tenantId, string name, Guid engagementId, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), WorkbookConsts.NameMaxLength, WorkbookConsts.NameMinLength);
            Name = name;

            EngagementId = engagementId;
            IsActive = isActive;
        }
    }
}