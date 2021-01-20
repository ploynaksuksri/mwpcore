using Volo.Abp.Application.Dtos;

namespace Mwp.Tenants.Dtos
{
    public class GetTenantTypesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}