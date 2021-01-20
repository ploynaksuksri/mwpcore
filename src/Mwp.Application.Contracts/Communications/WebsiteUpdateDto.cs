using System.ComponentModel.DataAnnotations;

namespace Mwp.Communications
{
    public class WebsiteUpdateDto
    {
        [Required]
        [StringLength(WebsiteConsts.NameMaxLength, MinimumLength = WebsiteConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}