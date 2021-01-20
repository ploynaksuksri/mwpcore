using System.Threading.Tasks;
using Mwp.DataSeeder;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Mwp.CloudService
{
    public class CloudServiceDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IMwpDataSeeder<IRepository<CloudService, int>, CloudService, int> _cloudServiceDataSeeder;
        private readonly IMwpDataSeeder<IRepository<CloudServiceLocation, int>, CloudServiceLocation, int> _cloudServiceLocationDataSeeder;
        private readonly IMwpDataSeeder<ICloudServiceOptionRepository, CloudServiceOption, int> _cloudServiceOptionDataSeeder;
        private readonly IMwpDataSeeder<IRepository<CloudServiceProvider, int>, CloudServiceProvider, int> _cloudServiceProviderDataSeeder;
        private readonly IMwpDataSeeder<IRepository<CloudServiceType, int>, CloudServiceType, int> _cloudServiceTypeDataSeeder;
        private readonly IMwpDataSeeder<ISharedResourceRepository, SharedResource> _sharedResourceDataSeeder;

        public CloudServiceDataSeedContributor(
            IMwpDataSeeder<IRepository<CloudService, int>, CloudService, int> cloudServiceDataSeeder,
            IMwpDataSeeder<IRepository<CloudServiceLocation, int>, CloudServiceLocation, int> cloudServiceLocationDataSeeder,
            IMwpDataSeeder<ICloudServiceOptionRepository, CloudServiceOption, int> cloudServiceOptionDataSeeder,
            IMwpDataSeeder<IRepository<CloudServiceProvider, int>, CloudServiceProvider, int> cloudServiceProviderDataSeeder,
            IMwpDataSeeder<IRepository<CloudServiceType, int>, CloudServiceType, int> cloudServiceTypeDataSeeder,
            IMwpDataSeeder<ISharedResourceRepository, SharedResource> sharedResourceDataSeeder)
        {
            _cloudServiceDataSeeder = cloudServiceDataSeeder;
            _cloudServiceLocationDataSeeder = cloudServiceLocationDataSeeder;
            _cloudServiceOptionDataSeeder = cloudServiceOptionDataSeeder;
            _cloudServiceProviderDataSeeder = cloudServiceProviderDataSeeder;
            _cloudServiceTypeDataSeeder = cloudServiceTypeDataSeeder;
            _sharedResourceDataSeeder = sharedResourceDataSeeder;
        }

        [UnitOfWork]
        public async Task SeedAsync(DataSeedContext context)
        {
            await _cloudServiceProviderDataSeeder.SeedData();
            await _cloudServiceLocationDataSeeder.SeedData();
            await _cloudServiceTypeDataSeeder.SeedData();
            await _cloudServiceDataSeeder.SeedData();
            await _cloudServiceOptionDataSeeder.SeedData();
            await _sharedResourceDataSeeder.SeedData();
        }
    }
}