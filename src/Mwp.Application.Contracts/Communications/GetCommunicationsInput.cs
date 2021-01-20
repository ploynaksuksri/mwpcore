using Volo.Abp.Application.Dtos;

namespace Mwp.Communications
{
    public class GetCommunicationsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}