using System.ComponentModel.DataAnnotations;

namespace Mwp.CloudService
{
    public enum CloudServiceLocations
    {
        [Display(Name = "southeastasia", ShortName = "sea", Description = "Southeast Asia")]
        SoutheastAsia = 1,

        [Display(Name = "australiasoutheast", ShortName = "ase", Description = "Australia Southeast")]
        AustraliaSoutheast = 2,

        [Display(Name = "australiaeast", ShortName = "aue", Description = "Australia East")]
        AustraliaEast = 3,

        [Display(Name = "uksouth", ShortName = "uks", Description = "UK South")]
        UKSouth = 4,

        [Display(Name = "ukwest", ShortName = "ukw", Description = "UK West")]
        UKWest = 5
    }
}