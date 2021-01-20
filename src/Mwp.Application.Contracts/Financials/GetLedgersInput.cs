using Volo.Abp.Application.Dtos;

namespace Mwp.Financials
{
    public class GetLedgersInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}