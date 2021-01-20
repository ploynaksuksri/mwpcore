using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Security.Encryption;

namespace Mwp.Tenants.Events.Result
{
    public class StorageProvisionResultEventHandler : ResourceProvisionResultEventHandler, IDistributedEventHandler<StorageProvisionResultEventData>, ITransientDependency
    {
        public StorageProvisionResultEventHandler(
            IRepository<TenantResource> tenantResourcesRepository,
            IStringEncryptionService encryptionService,
            ITenantResourceManager tenantResourceManager)
            : base(tenantResourcesRepository, encryptionService, tenantResourceManager)
        {
        }

        public async Task HandleEventAsync(StorageProvisionResultEventData provisionResult)
        {
            await ProcessResultAsync(provisionResult);
        }

        protected override async Task ProcessSuccessProvisioning(TenantResource resource, ResouceProvisionResultEventData provisionResult)
        {
            resource.ConnectionString = EncryptionService.Encrypt(provisionResult.ConnectionString);
            await TenantResourceManager.SetTenantIsActive(resource.TenantId, true);
        }
    }
}