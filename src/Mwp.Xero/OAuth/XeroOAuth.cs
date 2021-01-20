using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Mwp.ThirdPartyOAuth;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.UI.Navigation.Urls;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Token;
using XeroTenantModel = Xero.NetStandard.OAuth2.Models.Tenant;

namespace Mwp.Xero.OAuth
{
    public class XeroOAuth : IXeroOAuth, ITransientDependency
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public XeroOAuth(
            IConfiguration configuration,
            IAppUrlProvider webUrlService)
        {
            _clientId = configuration[XeroConsts.ClientId];
            _clientSecret = configuration[XeroConsts.ClientSecret];
            var selfUrl = webUrlService.GetUrlAsync(MwpConsts.AppName, MwpConsts.SelfUrl).Result.EnsureEndsWith('/');
            _redirectUri = $"{selfUrl}{XeroConsts.OAuthRedirectRoute}";
        }

        public string GetAuthoriseUrl(Guid mwpUserId, Guid mwpTenantId)
        {
            var client = GetXeroClient(mwpUserId, mwpTenantId);
            return client.BuildLoginUri();
        }

        public async Task<XeroToken> GetAccessTokenAsync(string code, string state)
        {
            var (mwpUserId, mwpTenantId) = OAuthHelper.ExtractState(state);

            var client = GetXeroClient(mwpUserId, mwpTenantId);
            var response = await client.RequestAccessTokenAsync(code);
            var xeroToken = MapToXeroToken(response);
            xeroToken.MwpUserId = mwpUserId;
            xeroToken.MwpTenantId = mwpTenantId;
            return xeroToken;
        }

        public async Task<List<XeroTenant>> GetConnectionsAsync(XeroToken xeroToken)
        {
            Check.NotNull(xeroToken, nameof(xeroToken));

            var client = GetXeroClient();
            var response = await client.GetConnectionsAsync(new XeroOAuth2Token
            {
                AccessToken = xeroToken.AccessToken,
                ExpiresAtUtc = xeroToken.ExpiresAtUtc,
                IdToken = xeroToken.IdToken,
                RefreshToken = xeroToken.RefreshToken
            });

            var xeroTenants = MapToXeroTenants(response);
            return xeroTenants;
        }

        public async Task<XeroToken> RefreshAccessToken(XeroToken currentXeroToken)
        {
            var client = GetXeroClient();
            var response = await client.RefreshAccessTokenAsync(new XeroOAuth2Token
            {
                AccessToken = currentXeroToken.AccessToken,
                ExpiresAtUtc = currentXeroToken.ExpiresAtUtc,
                IdToken = currentXeroToken.IdToken,
                RefreshToken = currentXeroToken.RefreshToken
            });
            return MapToXeroToken(response);
        }

        public async Task RemoveConnection(XeroToken xeroToken, XeroTenant xeroTenant)
        {
            var xeroAuth2Token = new XeroOAuth2Token
            {
                AccessToken = xeroToken.AccessToken,
                ExpiresAtUtc = xeroToken.ExpiresAtUtc,
                IdToken = xeroToken.IdToken,
                RefreshToken = xeroToken.RefreshToken
            };
            xeroAuth2Token.Tenants = new List<XeroTenantModel>
            {
                new XeroTenantModel
                {
                    TenantId = xeroTenant.XeroTenantId,
                    authEventId = xeroTenant.XeroConnection.AuthenticationEventId,
                    id = xeroTenant.XeroConnection.XeroConnectionId
                }
            };

            await GetXeroClient().DeleteConnectionAsync(xeroAuth2Token, xeroAuth2Token.Tenants.FirstOrDefault());
        }

        #region private

        private XeroClient GetXeroClient(Guid mwpUserId, Guid mwpTenantId)
        {
            return new XeroClient(GetXeroConfiguration(mwpUserId, mwpTenantId));
        }

        private XeroClient GetXeroClient()
        {
            return new XeroClient(GetXeroConfiguration());
        }

        private XeroConfiguration GetXeroConfiguration(Guid mwpUserId, Guid mwpTenantId)
        {
            var xeroConfig = GetXeroConfiguration();
            xeroConfig.State = OAuthHelper.GenerateState(mwpUserId, mwpTenantId);
            return xeroConfig;
        }

        private XeroConfiguration GetXeroConfiguration()
        {
            return new XeroConfiguration
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                CallbackUri = new Uri(_redirectUri),
                Scope = XeroConsts.DefaultMinimumScopes
            };
        }

        private XeroToken MapToXeroToken(IXeroToken token)
        {
            var xeroToken = new XeroToken
            {
                AccessToken = token.AccessToken,
                ExpiresAtUtc = token.ExpiresAtUtc,
                IdToken = token.IdToken,
                RefreshToken = token.RefreshToken
            };

            if (token.Tenants != null && token.Tenants.Any())
            {
                xeroToken.AuthenticationEventId = token.Tenants.First().authEventId;
            }

            return xeroToken;
        }

        private List<XeroTenant> MapToXeroTenants(List<XeroTenantModel> tenants)
        {
            var xeroTenants = new List<XeroTenant>();
            foreach (var tenant in tenants)
            {
                var connection = new XeroConnection
                {
                    XeroConnectionId = tenant.id,
                    AuthenticationEventId = tenant.authEventId,
                    XeroTenantId = tenant.TenantId,
                    IsConnected = true
                };
                var xeroTenant = new XeroTenant
                {
                    XeroTenantId = tenant.TenantId,
                    Name = tenant.TenantName,
                    TenantType = XeroTenantTypes.Orgnisation,
                    XeroConnection = connection
                };
                xeroTenants.Add(xeroTenant);
            }

            return xeroTenants;
        }

        #endregion private
    }
}