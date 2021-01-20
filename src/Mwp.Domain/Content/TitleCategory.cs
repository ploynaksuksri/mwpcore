using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Content
{
    public class TitleCategory : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }

        // 1 to 0..1 relationship
        public virtual ICollection<Title> Title { get; set; }

        public TitleCategory()
        {

        }

        public TitleCategory(Guid id, Guid? tenantId, string name, bool? isActive = null) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), TitleCategoryConsts.NameMaxLength, TitleCategoryConsts.NameMinLength);
            Name = name;

            IsActive = isActive;
        }
    }
}