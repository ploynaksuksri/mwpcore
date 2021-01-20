using System.ComponentModel.DataAnnotations;

namespace Mwp.Engagements
{
    public class DocumentCreateDto
    {
        [Required]
        [StringLength(DocumentConsts.NameMaxLength, MinimumLength = DocumentConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}