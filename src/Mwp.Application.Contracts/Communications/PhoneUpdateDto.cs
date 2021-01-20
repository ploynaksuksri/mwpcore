using System.ComponentModel.DataAnnotations;

namespace Mwp.Communications
{
    public class PhoneUpdateDto
    {
        [Required]
        [StringLength(PhoneConsts.NameMaxLength, MinimumLength = PhoneConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}