using System.ComponentModel.DataAnnotations;

namespace Mwp.Communications
{
    public class CommunicationCreateDto
    {
        [Required]
        [StringLength(CommunicationConsts.NameMaxLength, MinimumLength = CommunicationConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}