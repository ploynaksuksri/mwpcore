using System.ComponentModel.DataAnnotations;

namespace Mwp.Financials
{
    public class LedgerCreateDto
    {
        [Required]
        [StringLength(LedgerConsts.NameMaxLength, MinimumLength = LedgerConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}