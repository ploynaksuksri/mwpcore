using Volo.Abp.Application.Dtos;

namespace Mwp.Communications
{
    public class GetWebsitesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}