using Mwp.Extensions;
using Volo.Abp.Domain.Entities;

namespace Mwp.CloudService
{
    public class CloudServiceOption : Entity<int>
    {
        public int CloudServiceId { get; set; }
        public CloudService CloudService { get; set; }
        public string OptionName { get; set; }
        public string OptionDesc { get; set; }

        public bool IsShared { get; set; }
        public bool IsProvisionRequired { get; set; }

        protected CloudServiceOption()
        {
        }

        public CloudServiceOption(
            CloudServices cloudService,
            CloudServiceOptions cloudServiceOption,
            bool isShared,
            bool isProvisionRequired)
        {
            CloudServiceId = (int)cloudService;
            OptionName = cloudServiceOption.GetName();
            OptionDesc = cloudServiceOption.GetDescription();
            IsShared = isShared;
            IsProvisionRequired = isProvisionRequired;
        }
    }
}