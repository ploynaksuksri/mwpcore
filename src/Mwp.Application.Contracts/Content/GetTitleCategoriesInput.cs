using Volo.Abp.Application.Dtos;

namespace Mwp.Content
{
    public class GetTitleCategoriesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}