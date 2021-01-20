using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Mwp.Financials
{
    public class Account : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public const string DefaultSorting = "Name asc,CountryId asc,FullName asc,EmailAddress asc,PhoneNumber asc";

        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        public virtual string FullName { get; set; }

        [NotNull]
        public virtual string EmailAddress { get; set; }

        [NotNull]
        public virtual string PhoneNumber { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual Guid AccountId { get; set; }

        public virtual Account AccountNavigation { get; set; }

        public virtual Guid LedgerId { get; set; }

        public virtual Ledger Ledger { get; set; }

        public virtual ICollection<Account> InverseAccountNavigation { get; set; }

        public Account()
        {

        }

        public Account(Guid id, Guid? tenantId, [NotNull] string name, [NotNull] string fullName, [NotNull] string emailAddress, [NotNull] string phoneNumber, Guid accountId, Guid ledgerId) : base(id)
        {
            TenantId = tenantId;

            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), LedgerConsts.NameMaxLength, LedgerConsts.NameMinLength);
            Name = name;

            FullName = fullName;
            EmailAddress = emailAddress;
            PhoneNumber = phoneNumber;
            AccountId = accountId;
            LedgerId = ledgerId;
        }
    }
}