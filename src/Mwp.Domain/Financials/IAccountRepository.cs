using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Financials
{
    public interface IAccountRepository : IRepository<Account, Guid>
    {
        Task<List<Account>> GetListAsync(
            string filterText = null,
            string name = null,
            string fullName = null,
            string emailAddress = null,
            string phoneNumber = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            bool includeDetails = false,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            string fullName = null,
            string emailAddress = null,
            string phoneNumber = null,
            CancellationToken cancellationToken = default);
    }
}