using System.Collections.Generic;
using System.Linq;
using Mwp.DataSeeder;
using Volo.Abp.Domain.Repositories;

namespace Mwp.CloudService.DataSeeder
{
    public class CloudServiceTypeDataSeeder : MwpDataSeederBase<IRepository<CloudServiceType, int>, CloudServiceType, int>
    {
        public CloudServiceTypeDataSeeder(IRepository<CloudServiceType, int> repository)
            : base(repository)
        {
        }

        protected override List<CloudServiceType> BuildSeedingRecords()
        {
            return new List<CloudServiceType>
            {
                new CloudServiceType(CloudServiceTypes.General),
                new CloudServiceType(CloudServiceTypes.Compute),
                new CloudServiceType(CloudServiceTypes.Networking),
                new CloudServiceType(CloudServiceTypes.Storage),
                new CloudServiceType(CloudServiceTypes.Web),
                new CloudServiceType(CloudServiceTypes.Mobile),
                new CloudServiceType(CloudServiceTypes.Containers),
                new CloudServiceType(CloudServiceTypes.Databases),
                new CloudServiceType(CloudServiceTypes.Analytics),
                new CloudServiceType(CloudServiceTypes.AI),
                new CloudServiceType(CloudServiceTypes.InternetOfThings),
                new CloudServiceType(CloudServiceTypes.MixedReality),
                new CloudServiceType(CloudServiceTypes.Integration),
                new CloudServiceType(CloudServiceTypes.Identity),
                new CloudServiceType(CloudServiceTypes.Security),
                new CloudServiceType(CloudServiceTypes.Devops),
                new CloudServiceType(CloudServiceTypes.Migrate),
                new CloudServiceType(CloudServiceTypes.Management),
                new CloudServiceType(CloudServiceTypes.Intune),
                new CloudServiceType(CloudServiceTypes.Other)
            };
        }

        protected override CloudServiceType GetExistingRecord(List<CloudServiceType> existingRecords, CloudServiceType record)
        {
            return existingRecords.FirstOrDefault(e => e.ServiceTypeName == record.ServiceTypeName);
        }
    }
}