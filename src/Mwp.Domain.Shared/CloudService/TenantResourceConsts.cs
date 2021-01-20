namespace Mwp.CloudService
{
    public class TenantResourceConsts
    {
        public const int MaxNameLength = 50;
        public const int MaxUsernameLength = 1024;
        public const int MaxPasswordLength = 1024;
        public const int MaxAccessTokenLength = 1024;
        public const int MaxKeyLength = 1024;
        public const int MaxSubscriptionIdLength = 50;
        public const int MaxResourceGroupLength = 90;
        public const int MaxConnectionStringLength = 1024;
        public const int MaxServerNameLength = 100;
        public const int MaxStorageNameLength = 24;
        public const int MinStorageNameLength = 3;

        public const string ResourceNamePrefix = "mwp";

        public const string DefaultUserIdSetting = "DefaultDatabaseCredential:UserId";
        public const string DefaultPasswordSetting = "DefaultDatabaseCredential:Password";
        public const string AzureClientId = "Azure:ClientId";
        public const string AzureClientSecret = "Azure:ClientSecret";
        public const string AzureTenantId = "Azure:TenantId";

        public const string DbConnectionStringTemplate =
            "Server={server}.database.windows.net,1433;Initial Catalog={name};Persist Security Info=False;User ID={user_id};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30";

        public const string StorageConnectionStringTemplate = "DefaultEndpointsProtocol=https;AccountName={name};AccountKey={key};EndpointSuffix=core.windows.net";

        public const string DefaultSorting = "Name asc";
    }
}