using System.ComponentModel.DataAnnotations;

namespace Mwp.Entities
{
    public class EntityTypeCreateDto
    {
        [Required]
        [StringLength(EntityTypeConsts.NameMaxLength, MinimumLength = EntityTypeConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}