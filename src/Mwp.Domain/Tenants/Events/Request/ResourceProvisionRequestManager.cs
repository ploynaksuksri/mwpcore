using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;

namespace Mwp.Tenants.Events.Request
{
    public class ResourceProvisionRequestManager : DomainService, IResourceProvisionRequestManager
    {
        private readonly IDistributedEventBus _distributedEventBus;
        private readonly IJsonSerializer _jsonSerializer;

        public ResourceProvisionRequestManager(
            IDistributedEventBus distributedEventBus,
            IJsonSerializer jsonSerializer)
        {
            _distributedEventBus = distributedEventBus;
            _jsonSerializer = jsonSerializer;
        }

        public async Task ProvisionDatabase(TenantResource resource)
        {
            var request = new DatabaseProvisionRequestEventData
            {
                TenantId = resource.TenantId,
                ResourceId = resource.Id,
                LocationId = resource.CloudServiceLocationId,
                CloudServiceOptionId = resource.CloudServiceOptionId,
                ServerName = resource.ServerName,
                ElasticPoolName = resource.ElasticPoolName,
                ResourceGroupName = resource.ResourceGroup,
                SubscriptionId = resource.SubscriptionId,
                DatabaseName = resource.Name
            };
            Logger.LogInformation($"Provision Database message: {_jsonSerializer.Serialize(request)}");
            await _distributedEventBus.PublishAsync(request);
        }

        public async Task ProvisionStorage(TenantResource resource)
        {
            var request = new StorageProvisionRequestEventData
            {
                TenantId = resource.TenantId,
                ResourceId = resource.Id,
                LocationId = resource.CloudServiceLocationId,
                CloudServiceOptionId = resource.CloudServiceOptionId,
                ServerName = resource.ServerName,
                ResourceGroupName = resource.ResourceGroup,
                SubscriptionId = resource.SubscriptionId,
                StorageName = resource.Name
            };
            Logger.LogInformation($"Provision Storage message: {_jsonSerializer.Serialize(request)}");
            await _distributedEventBus.PublishAsync(request);
        }
    }
}