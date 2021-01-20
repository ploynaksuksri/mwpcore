using System.Collections.Generic;
using System.Linq;
using Mwp.DataSeeder;
using Volo.Abp.Domain.Repositories;

namespace Mwp.CloudService.DataSeeder
{
    public class CloudServiceLocationDataSeeder : MwpDataSeederBase<IRepository<CloudServiceLocation, int>, CloudServiceLocation, int>
    {
        public CloudServiceLocationDataSeeder(IRepository<CloudServiceLocation, int> repository)
            : base(repository)
        {
        }

        protected override List<CloudServiceLocation> BuildSeedingRecords()
        {
            return new List<CloudServiceLocation>
            {
                new CloudServiceLocation(CloudServiceLocations.SoutheastAsia),
                new CloudServiceLocation(CloudServiceLocations.AustraliaSoutheast),
                new CloudServiceLocation(CloudServiceLocations.AustraliaEast),
                new CloudServiceLocation(CloudServiceLocations.UKSouth),
                new CloudServiceLocation(CloudServiceLocations.UKWest)
            };
        }

        protected override CloudServiceLocation GetExistingRecord(List<CloudServiceLocation> existingRecords, CloudServiceLocation record)
        {
            return existingRecords.FirstOrDefault(e => e.LocationName == record.LocationName);
        }
    }
}