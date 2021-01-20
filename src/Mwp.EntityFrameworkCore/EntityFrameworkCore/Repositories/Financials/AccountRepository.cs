using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mwp.Financials;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Mwp.EntityFrameworkCore.Repositories.Financials
{
    public class AccountRepository : EfCoreRepository<MwpDbContext, Account, Guid>, IAccountRepository
    {
        public AccountRepository(IDbContextProvider<MwpDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Account>> GetListAsync(
            string filterText = null,
            string name = null,
            string fullName = null,
            string emailAddress = null,
            string phoneNumber = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            var query = includeDetails ? WithDetails() : DbSet;
            query = ApplyFilter(query, filterText, name, fullName, emailAddress, phoneNumber);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? Account.DefaultSorting : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            string fullName = null,
            string emailAddress = null,
            string phoneNumber = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(DbSet, filterText, name, fullName, emailAddress, phoneNumber);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Account> ApplyFilter(
            IQueryable<Account> query,
            string filterText,
            string name = null,
            string fullName = null,
            string emailAddress = null,
            string phoneNumber = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                    e => e.Name.Contains(filterText) ||
                         e.FullName.Contains(filterText) ||
                         e.EmailAddress.Contains(filterText) ||
                         e.PhoneNumber.Contains(filterText))
                .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                .WhereIf(!string.IsNullOrWhiteSpace(fullName), e => e.FullName.Contains(fullName))
                .WhereIf(!string.IsNullOrWhiteSpace(emailAddress), e => e.EmailAddress.Contains(emailAddress))
                .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber.Contains(phoneNumber));
        }
    }
}