using System.ComponentModel.DataAnnotations;

namespace Mwp.Entities
{
    public class RelationshipTypeCreateDto
    {
        [Required]
        [StringLength(RelationshipTypeConsts.NameMaxLength, MinimumLength = RelationshipTypeConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}