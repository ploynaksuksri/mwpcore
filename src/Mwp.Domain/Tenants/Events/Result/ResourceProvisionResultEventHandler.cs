using System.Threading.Tasks;
using Mwp.Provision;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Encryption;

namespace Mwp.Tenants.Events.Result
{
    public abstract class ResourceProvisionResultEventHandler
    {
        protected readonly IRepository<TenantResource> TenantResourcesRepository;
        protected readonly IStringEncryptionService EncryptionService;
        protected readonly ITenantResourceManager TenantResourceManager;

        protected ResourceProvisionResultEventHandler(
            IRepository<TenantResource> tenantResourcesRepository,
            IStringEncryptionService encryptionService,
            ITenantResourceManager tenantResourceManager
        )
        {
            TenantResourcesRepository = tenantResourcesRepository;
            EncryptionService = encryptionService;
            TenantResourceManager = tenantResourceManager;
        }

        public async Task ProcessResultAsync(ResouceProvisionResultEventData provisionResult)
        {
            var resource = await TenantResourcesRepository.GetAsync(e => e.Id == provisionResult.ResourceId);
            MapMessageToTenantResource(resource, provisionResult);

            if (resource.ProvisionStatus == ProvisionStatus.Success)
            {
                await ProcessSuccessProvisioning(resource, provisionResult);
            }

            await TenantResourcesRepository.UpdateAsync(resource, true);
        }

        protected virtual void MapMessageToTenantResource(TenantResource resource, ResouceProvisionResultEventData message)
        {
            resource.Name = message.Name;
            resource.ProvisionStatus = message.StatusId;
            resource.IsActive = resource.ProvisionStatus == ProvisionStatus.Success;
            resource.SubscriptionId = message.SubscriptionId;
            resource.ResourceGroup = message.ResourceGroupName;
            resource.ServerName = message.ServerName;
        }

        protected abstract Task ProcessSuccessProvisioning(TenantResource resource, ResouceProvisionResultEventData provisionResult);
    }
}