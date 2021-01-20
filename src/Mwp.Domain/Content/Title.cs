using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mwp.Engagements;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Content
{
    public class Title : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual Guid TitleCategoryId { get; set; }

        public virtual TitleCategory TitleCategory { get; set; }

        public virtual Guid WorkbookId { get; set; }

        public virtual Workbook Workbook { get; set; }

        public virtual Guid PublisherId { get; set; }

        public virtual Publisher Publisher { get; set; }

        public virtual ICollection<TitleAuthor> TitleAuthor { get; set; }

        public Title()
        {

        }

        public Title(Guid id, Guid? tenantId, string name, Guid titleCategoryId, Guid workbookId, Guid publisherId, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), TitleConsts.NameMaxLength, TitleConsts.NameMinLength);
            Name = name;

            TitleCategoryId = titleCategoryId;
            WorkbookId = workbookId;
            PublisherId = publisherId;
            IsActive = isActive;
        }
    }
}