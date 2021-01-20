using System.Net;
using Microsoft.Extensions.Localization;
using Mwp.Localization;
using Volo.Abp;
using Xero.NetStandard.OAuth2.Client;

namespace Mwp.Xero.Api
{
    public class XeroExceptionHandler
    {
        protected static IStringLocalizer<MwpResource> _l;

        public XeroExceptionHandler(IStringLocalizer<MwpResource> l)
        {
            _l = l;
        }

        public static UserFriendlyException ParseException(ApiException exception)
        {
            var userFriendlyException = (HttpStatusCode)exception.ErrorCode switch
            {
                HttpStatusCode.Forbidden => new UserFriendlyException(_l["XeroOrganisationNotConnected"]),
                HttpStatusCode.Unauthorized => new UserFriendlyException(_l["EnsureLoginToXero"]),
                _ => new UserFriendlyException(_l["ErrorConnectingToXero"])
            };

            return userFriendlyException;
        }
    }
}