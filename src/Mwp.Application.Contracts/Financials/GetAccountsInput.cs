using System;
using Volo.Abp.Application.Dtos;

namespace Mwp.Financials
{
    public class GetAccountsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? CountryId { get; set; }
    }
}