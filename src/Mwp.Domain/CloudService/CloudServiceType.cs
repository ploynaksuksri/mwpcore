using Mwp.Extensions;
using Volo.Abp.Domain.Entities;

namespace Mwp.CloudService
{
    public class CloudServiceType : Entity<int>
    {
        public string ServiceTypeName { get; set; }

        protected CloudServiceType()
        {
        }

        public CloudServiceType(CloudServiceTypes serviceType)
        {
            ServiceTypeName = serviceType.GetDescription();
        }
    }
}