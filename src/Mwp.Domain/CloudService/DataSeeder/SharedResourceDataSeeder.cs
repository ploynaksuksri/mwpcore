using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeeder;
using Mwp.SharedResource;

namespace Mwp.CloudService.DataSeeder
{
    public class SharedResourceDataSeeder : MwpDataSeederBase<ISharedResourceRepository, SharedResource>
    {
        public SharedResourceDataSeeder(ISharedResourceRepository repository)
            : base(repository)
        {
        }

        protected override List<SharedResource> BuildSeedingRecords()
        {
            return new List<SharedResource>
            {
                // Free trial
                CreateSharedResource(CloudServiceLocations.AustraliaEast, CloudServiceOptions.DatabaseBasic, SharedResourceSecrets.Aue.DatabaseBasic, true),
                CreateSharedResource(CloudServiceLocations.AustraliaEast, CloudServiceOptions.StorageStandard, SharedResourceSecrets.Aue.StorageStandard, true),
                CreateSharedResource(CloudServiceLocations.UKSouth, CloudServiceOptions.DatabaseBasic, SharedResourceSecrets.Uks.DatabaseBasic, true),
                CreateSharedResource(CloudServiceLocations.UKSouth, CloudServiceOptions.StorageStandard, SharedResourceSecrets.Uks.StorageStandard, true),

                // Commercial AUE
                CreateSharedResource(CloudServiceLocations.AustraliaEast, CloudServiceOptions.DatabaseBasic, SharedResourceSecrets.Aue.DatabaseBasic),
                CreateSharedResource(CloudServiceLocations.AustraliaEast, CloudServiceOptions.DatabaseStandard, SharedResourceSecrets.Aue.DatabaseStandard),
                CreateSharedResource(CloudServiceLocations.AustraliaEast, CloudServiceOptions.DatabaseAdvanced, SharedResourceSecrets.Aue.DatabaseAdvanced),
                CreateSharedResource(CloudServiceLocations.AustraliaEast, CloudServiceOptions.DatabasePremium, SharedResourceSecrets.Aue.DatabasePremium),
                CreateSharedResource(CloudServiceLocations.AustraliaEast, CloudServiceOptions.StorageStandard, SharedResourceSecrets.Aue.StorageStandard),
                CreateSharedResource(CloudServiceLocations.AustraliaEast, CloudServiceOptions.StoragePremium, SharedResourceSecrets.Aue.StoragePremium),

                // Commercial UKS
                CreateSharedResource(CloudServiceLocations.UKSouth, CloudServiceOptions.DatabaseBasic, SharedResourceSecrets.Uks.DatabaseBasic),
                CreateSharedResource(CloudServiceLocations.UKSouth, CloudServiceOptions.DatabaseStandard, SharedResourceSecrets.Uks.DatabaseStandard),
                CreateSharedResource(CloudServiceLocations.UKSouth, CloudServiceOptions.DatabaseAdvanced, SharedResourceSecrets.Uks.DatabaseAdvanced),
                CreateSharedResource(CloudServiceLocations.UKSouth, CloudServiceOptions.DatabasePremium, SharedResourceSecrets.Uks.DatabasePremium),
                CreateSharedResource(CloudServiceLocations.UKSouth, CloudServiceOptions.StorageStandard, SharedResourceSecrets.Uks.StorageStandard),
                CreateSharedResource(CloudServiceLocations.UKSouth, CloudServiceOptions.StoragePremium, SharedResourceSecrets.Uks.StoragePremium)
            };
        }

        protected override SharedResource GetExistingRecord(List<SharedResource> existingRecords, SharedResource record)
        {
            return existingRecords.FirstOrDefault(e => e.CloudServiceOptionId == record.CloudServiceOptionId
                                                       && e.CloudServiceLocationId == record.CloudServiceLocationId
                                                       && e.IsTrial == record.IsTrial);
        }

        public override async Task SeedData()
        {
            var existingRecords = await Repository.GetListAsync();

            foreach (var seedingRecord in SeedingRecords)
            {
                var existingRecord = GetExistingRecord(existingRecords, seedingRecord);
                if (existingRecord == null)
                {
                    await Repository.InsertAsync(seedingRecord, true);
                }
                else if (existingRecord.SecretName != seedingRecord.SecretName)
                {
                    existingRecord.SecretName = seedingRecord.SecretName;
                    await Repository.UpdateAsync(existingRecord, true);
                }
            }
        }

        private static SharedResource CreateSharedResource(CloudServiceLocations location, CloudServiceOptions option, string secretName, bool isTrial = false)
        {
            return new SharedResource
            {
                CloudServiceLocationId = (int)location,
                CloudServiceOptionId = (int)option,
                SecretName = secretName,
                IsTrial = isTrial
            };
        }
    }
}