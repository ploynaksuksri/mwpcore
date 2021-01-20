using System;

namespace Mwp.Wopi
{
    public class WopiActionDto
    {
        public Guid FileId { get; set; }

        public Guid FileIdInStorage { get; set; }

        public string BaseFileName { get; set; }

        public string UrlSrc { get; set; }

        public string AccessToken { get; set; }

        public long AccessTokenTtl { get; set; }
    }
}