using System.ComponentModel.DataAnnotations;

namespace Mwp.Entities
{
    public class EntityCreateDto
    {
        [Required]
        [StringLength(EntityConsts.NameMaxLength, MinimumLength = EntityConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}