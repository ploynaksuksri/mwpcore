using System;

namespace Mwp.Wopi
{
    public class WopiFileHistoryDto
    {
        public Guid Id { get; set; }

        public Guid WopiFileId { get; set; }

        public int Version { get; set; }

        public int Revision { get; set; }
        public Guid FileIdInStorage { get; set; }

        public string BaseFileName { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public string LastModificationDetail { get; set; }

        public string LastModifiedUsers { get; set; }
    }
}