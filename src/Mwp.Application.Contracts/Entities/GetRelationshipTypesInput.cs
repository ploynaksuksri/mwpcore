using Volo.Abp.Application.Dtos;

namespace Mwp.Entities
{
    public class GetRelationshipTypesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}