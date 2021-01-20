using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Wopi
{
    public class WopiFileHistory : AuditedEntity<Guid>
    {
        public Guid WopiFileId { get; set; }
        public int Version { get; set; }

        public int Revision { get; set; }

        public Guid FileIdInStorage { get; set; }

        public string BaseFileName { get; set; }

        public int Size { get; set; }


        public string LastModificationDetail { get; set; }

        public string LastModifiedUsers { get; set; }

        public string LastModifiedSessionId { get; set; }
    }
}