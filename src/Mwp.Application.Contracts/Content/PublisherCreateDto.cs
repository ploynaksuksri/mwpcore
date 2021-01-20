using System.ComponentModel.DataAnnotations;

namespace Mwp.Content
{
    public class PublisherCreateDto
    {
        [Required]
        [StringLength(PublisherConsts.NameMaxLength, MinimumLength = PublisherConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}