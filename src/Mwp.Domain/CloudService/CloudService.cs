using Mwp.Extensions;
using Volo.Abp.Domain.Entities;

namespace Mwp.CloudService
{
    public class CloudService : Entity<int>
    {
        public int CloudServiceProviderId { get; set; }

        public CloudServiceProvider CloudServiceProvider { get; set; }
        public string ServiceName { get; set; }

        public int CloudServiceTypeId { get; set; }

        public CloudServiceType CloudServiceType { get; set; }


        protected CloudService()
        {
        }

        public CloudService(
            CloudServiceTypes cloudServiceType,
            CloudServicePlatforms cloudServiceProvider,
            CloudServices service)
        {
            CloudServiceTypeId = (int)cloudServiceType;
            CloudServiceProviderId = (int)cloudServiceProvider;
            ServiceName = service.GetDescription();
        }
    }
}