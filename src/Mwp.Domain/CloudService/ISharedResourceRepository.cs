using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Mwp.CloudService
{
    public interface ISharedResourceRepository : IRepository<SharedResource>
    {
        Task<SharedResource> GetSharedResourceByOptionId(int optionId, int locationId, bool isFreeTrial = false);
    }
}