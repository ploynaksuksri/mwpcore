using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Mwp.Localization;
using Mwp.Permissions;
using Mwp.Utilities;
using Mwp.Xero.Dtos;
using Volo.Abp;

namespace Mwp.Xero
{
    public class XeroAuthAppService : MwpAppService, IXeroAuthAppService
    {
        protected IXeroAuthManager XeroAuthManager;
        protected readonly IStringLocalizer<MwpResource> _l;

        public XeroAuthAppService(IXeroAuthManager xeroAuthManager, IStringLocalizer<MwpResource> l)
        {
            XeroAuthManager = xeroAuthManager;
            _l = l;
        }

        [Authorize(MwpPermissions.Xero.Default)]
        public async Task<AuthoriseUrlDto> GetAuthoriseUrl()
        {
            return new AuthoriseUrlDto
            {
                Url = await XeroAuthManager.GetAuthoriseUrl((Guid)CurrentUser.Id, (Guid)CurrentTenant.Id)
            };
        }

        // This route name must match Redirect Uri set on Xero portal.
        [Route(XeroConsts.OAuthRedirectRoute)]
        [HttpGet]
        public async Task<IActionResult> GetAccessToken(string code, string state)
        {
            Logger.LogInformation($"Getting Xero Access Token from code: {code}, state: {state}");
            var xeroToken = await XeroAuthManager.GetAccessToken(code, state);
            Logger.LogInformation($"Got access token for user id {xeroToken.MwpUserId}, expires at {xeroToken.ExpiresAtUtc}");
            return GetConnectedContent();
        }

        [Authorize(MwpPermissions.Xero.Default)]
        public async Task<XeroTokenDto> RefreshAccessToken(Guid xeroTenantId)
        {
            Logger.LogInformation($"Refreshing Xero Access Token for MwpUserId: {CurrentUser.Id}, XeroTenantId: {xeroTenantId}");
            var newXeroToken = await XeroAuthManager.RefreshAccessToken((Guid)CurrentUser.Id, xeroTenantId);
            Logger.LogInformation($"Got refreshed Xero Access Token for MwpUserId: {CurrentUser.Id}, expires at {newXeroToken.ExpiresAtUtc}");
            return ObjectMapper.Map<XeroToken, XeroTokenDto>(newXeroToken);
        }

        [Authorize(MwpPermissions.Xero.Default)]
        public async Task RemoveConnection(string xeroTenantId)
        {
            Check.NotNullOrEmpty(xeroTenantId, nameof(xeroTenantId));
            Logger.LogInformation($"Removing connection of XeroTenantId: {xeroTenantId} from mwpUserId: {CurrentUser.Id}");
            await XeroAuthManager.RemoveConnection((Guid)CurrentUser.Id, new Guid(xeroTenantId));
        }

        public async Task<XeroTokenDto> GetCurrentToken(Guid xeroTenantId)
        {
            var currentToken = await XeroAuthManager.GetCurrentXeroToken((Guid)CurrentUser.Id, xeroTenantId);
            if (currentToken == null)
            {
                throw new UserFriendlyException(_l["EnsureConnectionToXero"]);
            }

            if (currentToken.IsAccessTokenExpired())
            {
                var newXeroToken = await RefreshAccessToken(xeroTenantId);
                return newXeroToken;
            }

            return ObjectMapper.Map<XeroToken, XeroTokenDto>(currentToken);
        }

        #region private methods

        private ContentResult GetConnectedContent()
        {
            var htmlContent = MwpEmbeddedResourceUtils.ReadStringFromEmbededResource(
                typeof(XeroTenantTypes).Assembly, XeroConsts.ConnectedHtmlFile);

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = htmlContent
            };
        }

        #endregion private methods
    }
}