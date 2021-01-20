using System.ComponentModel.DataAnnotations;

namespace Mwp.Entities
{
    public class EntityUpdateDto
    {
        [Required]
        [StringLength(EntityConsts.NameMaxLength, MinimumLength = EntityConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}