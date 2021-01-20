using System.ComponentModel.DataAnnotations;

namespace Mwp.Content
{
    public class TitleUpdateDto
    {
        [Required]
        [StringLength(TitleConsts.NameMaxLength, MinimumLength = TitleConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}