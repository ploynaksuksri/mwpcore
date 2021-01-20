using System.ComponentModel;

namespace Mwp.CloudService
{
    public enum CloudServiceTypes
    {
        [Description("General")]
        General = 1,

        [Description("Compute")]
        Compute = 2,

        [Description("Networking")]
        Networking = 3,

        [Description("Storage")]
        Storage = 4,

        [Description("Web")]
        Web = 5,

        [Description("Mobile")]
        Mobile = 6,

        [Description("Containers")]
        Containers = 7,

        [Description("Databases")]
        Databases = 8,

        [Description("Analytics")]
        Analytics = 9,

        [Description("AI + machine learning")]
        AI = 10,

        [Description("Internet of things")]
        InternetOfThings = 11,

        [Description("Mixed reality")]
        MixedReality = 12,

        [Description("Integration")]
        Integration = 13,

        [Description("Identity")]
        Identity = 14,

        [Description("Security")]
        Security = 15,

        [Description("DevOps")]
        Devops = 16,

        [Description("Migrate")]
        Migrate = 17,

        [Description("Management + governance")]
        Management = 18,

        [Description("Intune")]
        Intune = 19,

        [Description("Other")]
        Other = 20
    }
}