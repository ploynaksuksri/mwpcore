using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mwp.CloudService;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Mwp.EntityFrameworkCore.Repositories.CloudService
{
    public class SharedResourceRepository : EfCoreRepository<MwpDbContext, Mwp.CloudService.SharedResource>, ISharedResourceRepository
    {
        public SharedResourceRepository(IDbContextProvider<MwpDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<Mwp.CloudService.SharedResource> GetSharedResourceByOptionId(int optionId, int locationId, bool isFreeTrial = false)
        {
            return await DbSet
                .Include(e => e.CloudServiceOption)
                .FirstOrDefaultAsync(e => e.CloudServiceOptionId == optionId && e.CloudServiceLocationId == locationId && e.IsTrial == isFreeTrial);
        }
    }
}