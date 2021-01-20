using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Mwp.Tenants.Events.Request
{
    public interface IResourceProvisionRequestManager : IDomainService
    {
        Task ProvisionDatabase(TenantResource resource);

        Task ProvisionStorage(TenantResource resource);
    }
}