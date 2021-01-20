using System.ComponentModel.DataAnnotations;

namespace Mwp.CloudService
{
    public enum CloudServicePlatforms
    {
        [Display(Name = "Azure", GroupName = "Microsoft")]
        Azure = 1,

        [Display(Name = "Aws", GroupName = "Amazon")]
        Aws = 2,

        [Display(Name = "GCP", GroupName = "Google")]
        GoogleCloud = 3
    }
}