using System.ComponentModel.DataAnnotations;

namespace Mwp.Entities
{
    public class EntityGroupUpdateDto
    {
        [Required]
        [StringLength(EntityGroupConsts.NameMaxLength, MinimumLength = EntityGroupConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}