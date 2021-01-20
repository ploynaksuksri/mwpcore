using System.ComponentModel.DataAnnotations;

namespace Mwp.CloudService
{
    public enum CloudServiceOptions
    {
        [Display(Name = "Basic", Description = "Shared DB on single SQL database")]
        DatabaseBasic = 1,

        [Display(Name = "Standard", Description = "Dedicated DB on Elastic Pool (5 DTU)")]
        DatabaseStandard = 2,

        [Display(Name = "Advanced", Description = "Dedicated DB on Elastic Pool (10 DTU)")]
        DatabaseAdvanced = 3,

        [Display(Name = "Premium", Description = "Dedicated database server")]
        DatabasePremium = 4,

        [Display(Name = "Standard", Description = "Shared storage account")]
        StorageStandard = 5,

        [Display(Name = "Premium", Description = "Dedicated storage account")]
        StoragePremium = 6
    }
}