namespace Mwp.Qbo

{
    public class QboConsts
    {
        #region Configuration

        public const string ClientId = "QBO:ClientId";
        public const string ClientSecret = "QBO:ClientSecret";
        public const string Environment = "QBO:Environment";
        public const string BaseUrl = "QBO:BaseUrl";

        #endregion Configuration

        #region Auth constants

        public const string OAuthRedirectRoute = "QBO/AuthRedirect";
        public const string ConnectedHtmlFile = "Mwp.Qbo.QboConnected.html";
        public const string EnvironmentSandbox = "sandbox";
        public const string EnvironmentProduction = "production";

        #endregion Auth constants

        #region QBO entities related constraints

        public const int MaxTenantIdLength = 50;
        public const int MaxTenantNameLength = 200;
        public const int MaxAccessTokenLength = 4096;
        public const int MaxIdTokenLength = 4096;
        public const int MaxRefreshTokenLength = 512;

        #endregion QBO entities related constraints

        #region Reports

        public const string TrialBalanceReport = "TrialBalance";
        public const string GrandTotalRecord = "GrandTotal";

        #endregion Reports
    }
}