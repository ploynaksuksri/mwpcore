using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mwp.ThirdPartyOAuth
{
    public class OAuthToken : FullAuditedEntity<Guid>
    {
        public virtual string AccessToken { get; set; }
        public virtual string IdToken { get; set; }
        public virtual string RefreshToken { get; set; }
        public virtual DateTime ExpiresAtUtc { get; set; }
        public virtual bool IsRevoked { get; set; } = false;
        public virtual bool IsRefreshed { get; set; } = false;
        public virtual Guid MwpUserId { get; set; }

        #region Non EF columns

        public virtual bool IsAccessTokenExpired()
            => IsExpired(ExpiresAtUtc);

        protected virtual bool IsExpired(DateTime utcDateTime)
            => utcDateTime.Subtract(DateTime.UtcNow).TotalSeconds <= 60;

        [NotMapped]
        public virtual Guid MwpTenantId { get; set; }

        #endregion Non EF columns
    }
}