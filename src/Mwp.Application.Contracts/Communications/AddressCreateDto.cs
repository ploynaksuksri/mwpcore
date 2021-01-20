using System.ComponentModel.DataAnnotations;

namespace Mwp.Communications
{
    public class AddressCreateDto
    {
        [Required]
        [StringLength(AddressConsts.NameMaxLength, MinimumLength = AddressConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}