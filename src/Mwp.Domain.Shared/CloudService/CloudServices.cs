using System.ComponentModel;

namespace Mwp.CloudService
{
    /// <summary>
    ///     Value pf these enums must be the same as what we seeded in the CloudServiceDataSeeder
    /// </summary>
    public enum CloudServices
    {
        [Description("Azure App service")]
        AzureAppService = 1,

        [Description("Azure App service plan")]
        AzureAppServicePlan = 2,

        [Description("Azure SQL database")]
        AzureSQLDatabase = 3,

        [Description("Azure SQL server")]
        AzureSQLServer = 4,

        [Description("Azure SQL elastic pool")]
        AzureSQLElasticPool = 5,

        [Description("Azure Storage")]
        AzureStorage = 6
    }
}