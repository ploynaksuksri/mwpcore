using System.Collections.Generic;
using System.Linq;
using Mwp.DataSeeder;

namespace Mwp.CloudService.DataSeeder
{
    public class CloudServiceOptionDataSeeder : MwpDataSeederBase<ICloudServiceOptionRepository, CloudServiceOption, int>
    {
        public CloudServiceOptionDataSeeder(ICloudServiceOptionRepository repository)
            : base(repository)
        {
        }

        protected override List<CloudServiceOption> BuildSeedingRecords()
        {
            // Data order must follow order of CloudService list in the CloudServiceDataSeeder
            return new List<CloudServiceOption>
            {
                new CloudServiceOption(CloudServices.AzureSQLDatabase, CloudServiceOptions.DatabaseBasic, true, false),
                new CloudServiceOption(CloudServices.AzureSQLElasticPool, CloudServiceOptions.DatabaseStandard, true, true),
                new CloudServiceOption(CloudServices.AzureSQLElasticPool, CloudServiceOptions.DatabaseAdvanced, true, true),
                new CloudServiceOption(CloudServices.AzureSQLServer, CloudServiceOptions.DatabasePremium, false, true),
                new CloudServiceOption(CloudServices.AzureStorage, CloudServiceOptions.StorageStandard, true, false),
                new CloudServiceOption(CloudServices.AzureStorage, CloudServiceOptions.StoragePremium, false, true)
            };
        }

        protected override CloudServiceOption GetExistingRecord(List<CloudServiceOption> existingRecords, CloudServiceOption record)
        {
            return existingRecords.FirstOrDefault(e => e.CloudServiceId == record.CloudServiceId
                                                       && e.OptionName == record.OptionName
                                                       && e.OptionDesc == record.OptionDesc);
        }
    }
}