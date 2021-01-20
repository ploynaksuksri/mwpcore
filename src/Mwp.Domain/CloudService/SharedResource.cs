using System;
using Volo.Abp.Domain.Entities;

namespace Mwp.CloudService
{
    public class SharedResource : Entity<Guid>
    {
        public int CloudServiceLocationId { get; set; }
        public CloudServiceLocation CloudServiceLocation { get; set; }

        public int CloudServiceOptionId { get; set; }
        public CloudServiceOption CloudServiceOption { get; set; }

        public bool IsTrial { get; set; }

        public string SecretName { get; set; }
    }
}