using System;
using Mwp.Content;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Engagements
{
    public class WorkpaperComponent : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid WorkpaperId { get; set; }

        public virtual Guid ComponentId { get; set; }

        public virtual Workpaper Workpaper { get; set; }

        public virtual Component Component { get; set; }

        public WorkpaperComponent(Guid workpaperId, Guid componentId)
        {
            WorkpaperId = workpaperId;
            ComponentId = componentId;
        }
    }
}