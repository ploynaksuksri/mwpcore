using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.Extensions.Configuration;
using Mwp.Api;
using Mwp.Qbo;
using Mwp.ThirdPartyOAuth;
using Volo.Abp.DependencyInjection;
using Volo.Abp.UI.Navigation.Urls;

namespace Mwp.OAuth
{
    public class QboOAuth : IQboOAuth, ITransientDependency
    {
        private readonly OAuth2Client _authClient;
        private readonly QboQueryApi _queryApi;

        private readonly List<OidcScopes> _defaultScopes = new List<OidcScopes>
        {
            OidcScopes.Accounting
        };

        public QboOAuth(
            IConfiguration configuration,
            IAppUrlProvider webUrlService,
            QboQueryApi queryApi)
        {
            _queryApi = queryApi;

            var qboEnvironment = configuration[QboConsts.Environment] ?? QboConsts.EnvironmentProduction;
            _authClient = new OAuth2Client(
                configuration[QboConsts.ClientId],
                configuration[QboConsts.ClientSecret],
                BuildRedirectUri(webUrlService),
                qboEnvironment);
        }

        public string GetAuthoriseUrl(Guid mwpUserId, Guid mwpTenantId)
        {
            var state = OAuthHelper.GenerateState(mwpUserId, mwpTenantId);
            return _authClient.GetAuthorizationURL(_defaultScopes, state);
        }

        public async Task<Tuple<QboToken, QboTenant>> GetAccessTokenAsync(string code, string state, string realmId)
        {
            var tokenResponse = await _authClient.GetBearerTokenAsync(code);
            var qboToken = MapToQboToken(tokenResponse, state, realmId);
            var company = _queryApi.GetCompanyInfo(realmId, qboToken);
            return new Tuple<QboToken, QboTenant>(qboToken, company);
        }

        public async Task<QboToken> RefreshAccessToken(QboToken currentToken)
        {
            var tokenResponse = await _authClient.RefreshTokenAsync(currentToken.RefreshToken);
            var qboToken = MapToQboToken(tokenResponse);
            qboToken.MwpTenantId = currentToken.MwpTenantId;
            qboToken.MwpUserId = currentToken.MwpUserId;
            qboToken.QboTenantId = currentToken.QboTenantId;
            return qboToken;
        }

        public async Task RemoveConnection(QboToken token)
        {
            var response = await _authClient.RevokeTokenAsync(token.RefreshToken);
            if (response.IsError)
            {
                throw new Exception("Failed to revoke token.");
            }
        }

        #region private methods

        private string BuildRedirectUri(IAppUrlProvider webUrlService)
        {
            var selfUrl = webUrlService.GetUrlAsync(MwpConsts.AppName, MwpConsts.SelfUrl).Result.EnsureEndsWith('/');
            var redirectUri = $"{selfUrl}{QboConsts.OAuthRedirectRoute}";
            return redirectUri;
        }

        private DateTime CalculateUTCDateTimeFromSeconds(long second)
        {
            return DateTime.UtcNow.AddSeconds(second);
        }

        private QboToken MapToQboToken(TokenResponse tokenResponse, string state, string companyId)
        {
            var qboToken = MapToQboToken(tokenResponse);

            var (mwpUserId, mwpTenantId) = OAuthHelper.ExtractState(state);
            qboToken.MwpUserId = mwpUserId;
            qboToken.QboTenantId = companyId;
            qboToken.MwpTenantId = mwpTenantId;
            return qboToken;
        }

        private QboToken MapToQboToken(TokenResponse tokenResponse)
        {
            return new QboToken
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                IdToken = tokenResponse.IdentityToken,
                ExpiresAtUtc = CalculateUTCDateTimeFromSeconds(tokenResponse.AccessTokenExpiresIn),
                RefreshTokenExpiresAtUtc = CalculateUTCDateTimeFromSeconds(tokenResponse.RefreshTokenExpiresIn)
            };
        }

        #endregion private methods
    }
}