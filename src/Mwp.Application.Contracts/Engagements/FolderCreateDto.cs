using System.ComponentModel.DataAnnotations;

namespace Mwp.Engagements
{
    public class FolderCreateDto
    {
        [Required]
        [StringLength(FolderConsts.NameMaxLength, MinimumLength = FolderConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}