using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.Content
{
    public class TitleAuthor : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid TitleId { get; set; }

        public virtual Guid AuthorId { get; set; }

        public virtual Title Title { get; set; }

        public virtual Author Author { get; set; }

        public TitleAuthor(Guid titleId, Guid authorId)
        {
            TitleId = titleId;
            AuthorId = authorId;
        }
    }
}