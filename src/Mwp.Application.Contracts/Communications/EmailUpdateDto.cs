using System.ComponentModel.DataAnnotations;

namespace Mwp.Communications
{
    public class EmailUpdateDto
    {
        [Required]
        [StringLength(EmailConsts.NameMaxLength, MinimumLength = EmailConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}