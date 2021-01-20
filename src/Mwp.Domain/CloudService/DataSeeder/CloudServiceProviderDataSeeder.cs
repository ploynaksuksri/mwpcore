using System.Collections.Generic;
using System.Linq;
using Mwp.DataSeeder;
using Volo.Abp.Domain.Repositories;

namespace Mwp.CloudService.DataSeeder
{
    public class CloudServiceProviderDataSeeder : MwpDataSeederBase<IRepository<CloudServiceProvider, int>, CloudServiceProvider, int>
    {
        public CloudServiceProviderDataSeeder(IRepository<CloudServiceProvider, int> repository)
            : base(repository)
        {
        }

        protected override List<CloudServiceProvider> BuildSeedingRecords()
        {
            return new List<CloudServiceProvider>
            {
                new CloudServiceProvider(CloudServicePlatforms.Azure),
                new CloudServiceProvider(CloudServicePlatforms.Aws),
                new CloudServiceProvider(CloudServicePlatforms.GoogleCloud)
            };
        }

        protected override CloudServiceProvider GetExistingRecord(List<CloudServiceProvider> existingRecords, CloudServiceProvider record)
        {
            return existingRecords.FirstOrDefault(e => e.PlatformName == record.PlatformName
                                                       && e.CompanyName == record.CompanyName);
        }
    }
}