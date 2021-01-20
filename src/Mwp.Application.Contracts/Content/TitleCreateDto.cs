using System.ComponentModel.DataAnnotations;

namespace Mwp.Content
{
    public class TitleCreateDto
    {
        [Required]
        [StringLength(TitleConsts.NameMaxLength, MinimumLength = TitleConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}