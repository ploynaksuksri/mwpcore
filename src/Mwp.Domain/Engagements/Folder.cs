using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Engagements
{
    public class Folder : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        public virtual Guid ParentFolderId { get; set; }

        public virtual Folder Parent { get; set; }

        public virtual Guid WorkbookId { get; set; }

        public virtual Workbook Workbook { get; set; }

        public virtual ICollection<Folder> Folders { get; set; }

        public virtual ICollection<Workpaper> Workpapers { get; set; }

        public Folder()
        {

        }

        public Folder(Guid id, Guid? tenantId, string name, Guid parentFolderId, Guid workbookId, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), FolderConsts.NameMaxLength, FolderConsts.NameMinLength);
            Name = name;

            ParentFolderId = parentFolderId;
            WorkbookId = workbookId;
            IsActive = isActive;
        }
    }
}