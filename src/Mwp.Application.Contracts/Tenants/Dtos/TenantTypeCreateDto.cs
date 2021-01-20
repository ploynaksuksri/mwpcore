using System.ComponentModel.DataAnnotations;

namespace Mwp.Tenants.Dtos
{
    public class TenantTypeCreateDto
    {
        [Required]
        [StringLength(TenantTypeConsts.NameMaxLength, MinimumLength = TenantTypeConsts.NameMinLength)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}