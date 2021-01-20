using System.ComponentModel.DataAnnotations;

namespace Mwp.Content
{
    public class TemplateCreateDto
    {
        [Required]
        [StringLength(TemplateConsts.NameMaxLength, MinimumLength = TemplateConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}