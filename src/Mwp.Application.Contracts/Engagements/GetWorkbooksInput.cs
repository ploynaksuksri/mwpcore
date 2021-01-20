using Volo.Abp.Application.Dtos;

namespace Mwp.Engagements
{
    public class GetWorkbooksInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}