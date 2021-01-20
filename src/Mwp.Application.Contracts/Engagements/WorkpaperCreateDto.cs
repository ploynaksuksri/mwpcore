using System.ComponentModel.DataAnnotations;

namespace Mwp.Engagements
{
    public class WorkpaperCreateDto
    {
        [Required]
        [StringLength(WorkpaperConsts.NameMaxLength, MinimumLength = WorkpaperConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}