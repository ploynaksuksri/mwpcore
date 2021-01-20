using System;
using System.ComponentModel.DataAnnotations;

namespace Mwp.Financials
{
    public class AccountUpdateDto
    {
        [Required]
        [StringLength(AccountConsts.NameMaxLength, MinimumLength = AccountConsts.NameMinLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AccountConsts.FullNameMaxLength, MinimumLength = AccountConsts.FullNameMinLength)]
        public string FullName { get; set; }

        [Required]
        [StringLength(AccountConsts.EmailAddressMaxLength, MinimumLength = AccountConsts.EmailAddressMinLength)]
        public string EmailAddress { get; set; }

        [StringLength(AccountConsts.PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; }

        [Required]
        public Guid CountryId { get; set; }
    }
}