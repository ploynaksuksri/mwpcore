namespace Mwp.Tenants
{
    public class TenantConsts
    {
        public const string IsActive = "IsActive";
        public const string TenantParentId = "TenantParentId";

        public const string ProvisionServiceConnectionString = "ProvisionServiceBus:ConnectionString";
        public const string DatabaseCreatingQueue = "ProvisionServiceBus:DatabaseCreatingQueue";
        public const string StorageCreatingQueue = "ProvisionServiceBus:StorageCreatingQueue";
        public const string DatabaseCreatedQueue = "ProvisionServiceBus:DatabaseCreatedQueue";
        public const string StorageCreatedQueue = "ProvisionServiceBus:StorageCreatedQueue";

        public const string AdminEmailProperty = "AdminEmail";
        public const string AdminPasswordProperty = "AdminPassword";
        public const string DefaultAdminEmail = "admin@myworkpapers.com";
    }
}