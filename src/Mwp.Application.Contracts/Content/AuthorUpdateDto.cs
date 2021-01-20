using System.ComponentModel.DataAnnotations;

namespace Mwp.Content
{
    public class AuthorUpdateDto
    {
        [Required]
        [StringLength(AuthorConsts.NameMaxLength, MinimumLength = AuthorConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}