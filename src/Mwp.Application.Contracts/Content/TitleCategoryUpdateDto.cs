using System.ComponentModel.DataAnnotations;

namespace Mwp.Content
{
    public class TitleCategoryUpdateDto
    {
        [Required]
        [StringLength(TitleCategoryConsts.NameMaxLength, MinimumLength = TitleCategoryConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}