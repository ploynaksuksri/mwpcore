using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Wopi
{
    public interface IWopiFileHistoryRepository : IRepository<WopiFileHistory, Guid>
    {
        Task<WopiFileHistory> GetLatestHistory(Guid FileId, CancellationToken cancellationToken = default);
    }
}