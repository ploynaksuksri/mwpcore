namespace Mwp.Xero
{
    public class XeroConsts
    {
        #region Configuration

        public const string ClientId = "Xero:ClientId";
        public const string ClientSecret = "Xero:ClientSecret";

        #endregion Configuration

        #region Auth Constants

        public const string SignInRoute = "/Xero/Signin";
        public const string OAuthRedirectRoute = "Xero/AuthRedirect";
        public const string CodeResponseType = "code";
        public const string AuthorisationCodeGrantType = "authorization_code";
        public const string RefreshTokenGrantType = "refresh_token";
        public const string DefaultMinimumScopes = "openid profile email offline_access accounting.reports.read accounting.transactions";
        public const string ConnectedHtmlFile = "Mwp.Xero.XeroConnected.html";

        #endregion Auth Constants

        #region Xero entities related constraints

        public const int MaxTenantNameLength = 200;
        public const int MaxAccessTokenLength = 2048;
        public const int MaxIdTokenLength = 2048;
        public const int MaxRefreshTokenLength = 512;

        #endregion Xero entities related constraints
    }
}