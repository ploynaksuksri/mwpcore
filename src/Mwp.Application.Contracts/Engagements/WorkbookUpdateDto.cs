using System.ComponentModel.DataAnnotations;

namespace Mwp.Engagements
{
    public class WorkbookUpdateDto
    {
        [Required]
        [StringLength(WorkbookConsts.NameMaxLength, MinimumLength = WorkbookConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}