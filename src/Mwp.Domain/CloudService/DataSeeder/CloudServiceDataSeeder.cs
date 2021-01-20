using System.Collections.Generic;
using System.Linq;
using Mwp.DataSeeder;
using Volo.Abp.Domain.Repositories;

namespace Mwp.CloudService.DataSeeder
{
    public class CloudServiceDataSeeder : MwpDataSeederBase<IRepository<CloudService, int>, CloudService, int>
    {
        public CloudServiceDataSeeder(IRepository<CloudService, int> repository)
            : base(repository)
        {
        }

        protected override List<CloudService> BuildSeedingRecords()
        {
            return new List<CloudService>
            {
                new CloudService(CloudServiceTypes.Compute, CloudServicePlatforms.Azure, CloudServices.AzureAppService), // id = 1
                new CloudService(CloudServiceTypes.Compute, CloudServicePlatforms.Azure, CloudServices.AzureAppServicePlan), // id = 2
                new CloudService(CloudServiceTypes.Databases, CloudServicePlatforms.Azure, CloudServices.AzureSQLDatabase), // id = 3
                new CloudService(CloudServiceTypes.Databases, CloudServicePlatforms.Azure, CloudServices.AzureSQLServer), // id = 4
                new CloudService(CloudServiceTypes.Databases, CloudServicePlatforms.Azure, CloudServices.AzureSQLElasticPool), // id = 5
                new CloudService(CloudServiceTypes.Storage, CloudServicePlatforms.Azure, CloudServices.AzureStorage) // id = 6
            };
        }

        protected override CloudService GetExistingRecord(List<CloudService> existingRecords, CloudService record)
        {
            return existingRecords.FirstOrDefault(e => e.CloudServiceProviderId == record.CloudServiceProviderId
                                                       && e.CloudServiceTypeId == record.CloudServiceTypeId
                                                       && e.ServiceName == record.ServiceName);
        }
    }
}