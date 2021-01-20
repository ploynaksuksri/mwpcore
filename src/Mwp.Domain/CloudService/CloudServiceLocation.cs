using Mwp.Extensions;
using Volo.Abp.Domain.Entities;

namespace Mwp.CloudService
{
    public class CloudServiceLocation : Entity<int>
    {
        public string LocationName { get; set; }

        protected CloudServiceLocation()
        {
        }

        public CloudServiceLocation(CloudServiceLocations location)
        {
            LocationName = location.GetDescription();
        }
    }
}