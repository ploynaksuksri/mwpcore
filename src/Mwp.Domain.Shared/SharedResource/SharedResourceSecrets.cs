namespace Mwp.SharedResource
{
    public class SharedResourceSecrets
    {
        public const string DatabaseBasic = "db-basic";
        public const string DatabaseStandard = "db-standard";
        public const string DatabaseAdvanced = "db-advanced";
        public const string DatabasePremium = "db-premium";
        public const string StorageStandard = "storage-standard";
        public const string StoragePremium = "storage-premium";

        public class Aue
        {
            public const string RegionSuffix = "-aue";
            public const string DatabaseBasic = SharedResourceSecrets.DatabaseBasic + RegionSuffix;
            public const string DatabaseStandard = SharedResourceSecrets.DatabaseStandard + RegionSuffix;
            public const string DatabaseAdvanced = SharedResourceSecrets.DatabaseAdvanced + RegionSuffix;
            public const string DatabasePremium = SharedResourceSecrets.DatabasePremium + RegionSuffix;
            public const string StorageStandard = SharedResourceSecrets.StorageStandard + RegionSuffix;
            public const string StoragePremium = SharedResourceSecrets.StoragePremium + RegionSuffix;
        }

        public static class Uks
        {
            public const string RegionSuffix = "-uks";
            public const string DatabaseBasic = SharedResourceSecrets.DatabaseBasic + RegionSuffix;
            public const string DatabaseStandard = SharedResourceSecrets.DatabaseStandard + RegionSuffix;
            public const string DatabaseAdvanced = SharedResourceSecrets.DatabaseAdvanced + RegionSuffix;
            public const string DatabasePremium = SharedResourceSecrets.DatabasePremium + RegionSuffix;
            public const string StorageStandard = SharedResourceSecrets.StorageStandard + RegionSuffix;
            public const string StoragePremium = SharedResourceSecrets.StoragePremium + RegionSuffix;
        }
    }
}