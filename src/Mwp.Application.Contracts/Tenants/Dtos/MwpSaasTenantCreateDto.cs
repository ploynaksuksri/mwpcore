using Volo.Saas.Host.Dtos;

namespace Mwp.Tenants.Dtos
{
    public class MwpSaasTenantCreateDto : SaasTenantCreateDto
    {
        public int LocationId { get; set; }
        public int? DatabaseOptionId { get; set; }
        public int? StorageOptionId { get; set; }
    }
}