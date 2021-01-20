using System.ComponentModel.DataAnnotations;

namespace Mwp.Content
{
    public class ComponentUpdateDto
    {
        [Required]
        [StringLength(ComponentConsts.NameMaxLength, MinimumLength = ComponentConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}