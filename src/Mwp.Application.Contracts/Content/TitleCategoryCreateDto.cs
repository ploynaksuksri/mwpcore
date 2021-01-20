using System.ComponentModel.DataAnnotations;

namespace Mwp.Content
{
    public class TitleCategoryCreateDto
    {
        [Required]
        [StringLength(TitleCategoryConsts.NameMaxLength, MinimumLength = TitleCategoryConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}