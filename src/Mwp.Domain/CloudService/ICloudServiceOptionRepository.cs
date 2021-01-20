using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Mwp.CloudService
{
    public interface ICloudServiceOptionRepository : IRepository<CloudServiceOption, int>
    {
        Task<List<CloudServiceOption>> GetOptionsByServiceType(CloudServiceTypes type,
            CancellationToken cancellationToken = default);
    }
}