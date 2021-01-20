using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Mwp.Localization;

namespace Mwp.Financials
{
    public class AccountCreateDto : IValidatableObject
    {

        [Required]
        [StringLength(AccountConsts.NameMaxLength, MinimumLength = AccountConsts.NameMinLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AccountConsts.FullNameMaxLength, MinimumLength = AccountConsts.FullNameMinLength)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AccountConsts.EmailAddressMaxLength, MinimumLength = AccountConsts.EmailAddressMinLength)]
        public string EmailAddress { get; set; }

        [StringLength(AccountConsts.PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; }

        [Required]
        public Guid CountryId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var l = validationContext.GetRequiredService<IStringLocalizer<MwpResource>>();

            var isValidEmail = Regex.IsMatch(EmailAddress, ApplicationContractsConsts.EmailRegex, RegexOptions.IgnoreCase);

            if (EmailAddress.IsNullOrWhiteSpace() || !isValidEmail)
            {
                yield return new ValidationResult(l["InvalidEmailAddress"], new[] { nameof(EmailAddress) });
            }
        }
    }
}