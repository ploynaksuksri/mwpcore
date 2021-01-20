using Volo.Abp.Application.Dtos;

namespace Mwp.Content
{
    public class GetAuthorsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}