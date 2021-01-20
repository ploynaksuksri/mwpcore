using Volo.Saas.Host.Dtos;

namespace Mwp.Tenants.Dtos
{
    public class MwpSaasTenantDto : SaasTenantDto
    {
        public int? LocationId { get; set; }
        public string LocationName { get; set; }

        public int? DatabaseOptionId { get; set; }
        public string DatabaseOptionName { get; set; }

        public int? StorageOptionId { get; set; }
        public string StorageOptionName { get; set; }
    }
}