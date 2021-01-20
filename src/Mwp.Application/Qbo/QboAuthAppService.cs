using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Mwp.Localization;
using Mwp.Permissions;
using Mwp.Qbo.Dtos;
using Mwp.Utilities;
using Mwp.Xero.Dtos;
using Volo.Abp;

namespace Mwp.Qbo
{
    public class QboAuthAppService : MwpAppService, IQboAuthAppService
    {
        private readonly IQboAuthManager _authManager;
        protected readonly IStringLocalizer<MwpResource> _l;

        public QboAuthAppService(IQboAuthManager authManager, IStringLocalizer<MwpResource> l)
        {
            _authManager = authManager;
            _l = l;
        }

        [Route(QboConsts.OAuthRedirectRoute)]
        [HttpGet]
        public async Task<IActionResult> GetAccessToken(string code, string state, string realmId)
        {
            await _authManager.GetAccessToken(code, state, realmId);
            return GetConnectedContent();
        }

        [Authorize(MwpPermissions.Qbo.Default)]
        public async Task<AuthoriseUrlDto> GetAuthoriseUrl()
        {
            var tenantId = CurrentTenant.Id.HasValue ? CurrentTenant.Id.Value : Guid.Empty;
            return new AuthoriseUrlDto
            {
                Url = await _authManager.GetAuthoriseUrl(CurrentUser.Id.Value, tenantId)
            };
        }

        [Authorize(MwpPermissions.Qbo.Default)]
        public async Task<QboTokenDto> GetCurrentToken(string companyId)
        {
            var currentToken = await _authManager.GetCurrentToken(CurrentUser.Id, companyId);
            if (currentToken == null)
            {
                throw new UserFriendlyException(_l["EnsureConnectionToQbo"]);
            }

            if (currentToken.IsAccessTokenExpired())
            {
                if (currentToken.IsRefreshTokenExpired())
                {
                    throw new UserFriendlyException(_l["QboSessionExpired"]);
                }

                return await RefreshAccessToken(companyId);
            }

            return ObjectMapper.Map<QboToken, QboTokenDto>(currentToken);
        }

        [Authorize(MwpPermissions.Qbo.Default)]
        public async Task<QboTokenDto> RefreshAccessToken(string companyId)
        {
            var newToken = await _authManager.RefreshAccessToken(CurrentUser.Id.Value, companyId);
            return ObjectMapper.Map<QboToken, QboTokenDto>(newToken);
        }

        [Authorize(MwpPermissions.Qbo.Default)]
        public async Task RemoveConnection(string companyId)
        {
            await _authManager.RemoveConnection(CurrentUser.Id.Value, companyId);
        }

        #region private methods

        private ContentResult GetConnectedContent()
        {
            var htmlContent = MwpEmbeddedResourceUtils.ReadStringFromEmbededResource(
                typeof(QboConsts).Assembly, QboConsts.ConnectedHtmlFile);

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