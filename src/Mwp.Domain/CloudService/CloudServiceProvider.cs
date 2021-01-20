using Mwp.Extensions;
using Volo.Abp.Domain.Entities;

namespace Mwp.CloudService
{
    public class CloudServiceProvider : Entity<int>
    {
        public string PlatformName { get; set; }

        public string CompanyName { get; set; }

        protected CloudServiceProvider()
        {
        }

        public CloudServiceProvider(CloudServicePlatforms cloudServiceProvider)
        {
            PlatformName = cloudServiceProvider.GetName();
            CompanyName = cloudServiceProvider.GetGroupName();
        }
    }
}