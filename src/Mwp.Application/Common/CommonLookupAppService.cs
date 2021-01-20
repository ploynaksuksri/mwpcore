using System.Collections.Generic;
using System.Threading.Tasks;
using Mwp.CloudService;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Saas.Editions;

namespace Mwp.Common
{
    public class CommonLookupAppService : ApplicationService
    {
        protected IRepository<CloudServiceLocation, int> CloudServiceLocationRepository;
        protected ICloudServiceOptionRepository CloudServiceOptionRepository;
        protected IEditionRepository EditionRepository;

        public CommonLookupAppService(
            IRepository<CloudServiceLocation, int> cloudServiceLocationRepository,
            ICloudServiceOptionRepository cloudServiceOptionRepository,
            IEditionRepository editionRepository)
        {
            CloudServiceLocationRepository = cloudServiceLocationRepository;
            CloudServiceOptionRepository = cloudServiceOptionRepository;
            EditionRepository = editionRepository;
        }

        public async Task<List<CloudServiceLocation>> GetLocations()
        {
            return await CloudServiceLocationRepository.GetListAsync();
        }

        public async Task<List<CloudServiceOption>> GetDatabaseOptions()
        {
            return await CloudServiceOptionRepository.GetOptionsByServiceType(CloudServiceTypes.Databases);
        }

        public async Task<List<CloudServiceOption>> GetStorageOptions()
        {
            return await CloudServiceOptionRepository.GetOptionsByServiceType(CloudServiceTypes.Storage);
        }

        public async Task<List<Edition>> GetEditions()
        {
            using (CurrentTenant.Change(null))
            {
                return await EditionRepository.GetListAsync();
            }
        }
    }
}