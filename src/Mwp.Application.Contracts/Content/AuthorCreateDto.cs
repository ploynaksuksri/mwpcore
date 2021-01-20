using System.ComponentModel.DataAnnotations;

namespace Mwp.Content
{
    public class AuthorCreateDto
    {
        [Required]
        [StringLength(AuthorConsts.NameMaxLength, MinimumLength = AuthorConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}