using System.ComponentModel.DataAnnotations;

namespace Mwp.Engagements
{
    public class EngagementUpdateDto
    {
        [Required]
        [StringLength(EngagementConsts.NameMaxLength, MinimumLength = EngagementConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}