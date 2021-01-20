using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mwp.CloudService;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Mwp.EntityFrameworkCore.Repositories.CloudService
{
    public class CloudServiceOptionRepository : EfCoreRepository<MwpDbContext, CloudServiceOption, int>, ICloudServiceOptionRepository
    {
        public CloudServiceOptionRepository(IDbContextProvider<MwpDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<List<CloudServiceOption>> GetOptionsByServiceType(CloudServiceTypes type, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(e => e.CloudService.CloudServiceTypeId == (int)type)
                .ToListAsync(cancellationToken);
        }
    }
}