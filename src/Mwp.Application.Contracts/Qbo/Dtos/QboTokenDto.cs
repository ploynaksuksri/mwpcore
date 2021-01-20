using System;

namespace Mwp.Qbo.Dtos
{
    public class QboTokenDto
    {
        public Guid MwpUserId { get; set; }
        public string AccessToken { get; set; }
    }
}