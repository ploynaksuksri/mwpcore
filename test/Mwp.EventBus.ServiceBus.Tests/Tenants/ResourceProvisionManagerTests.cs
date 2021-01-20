using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.DependencyInjection;
using Mwp.CloudService;
using Mwp.Configuration;
using Mwp.EventBus.ServiceBus;
using Mwp.Provision;
using Mwp.Tenants;
using Mwp.Tenants.Events.Request;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Mwp.ServiceBus.Tenants
{
    public class ResourceProvisionManagerTests : MwpEventBusServiceBusTestBase
    {
        public const string SkipReasonForManualRun = "We do not use ServiceBus for now, so just skip these tests to save running time";

        private readonly IResourceProvisionRequestManager _resourceProvisionRequestManager;

        private string _serviceBusConnectionString;
        private string _databaseCreatingQueue;
        private string _storageCreatingQueue;

        public ResourceProvisionManagerTests()
        {
            _resourceProvisionRequestManager = GetRequiredService<ResourceProvisionRequestManager>();
        }

        protected override void BeforeAddApplication(IServiceCollection services)
        {
            var configuration = ConfigurationUtils.BuildConfiguration();

            services.ReplaceConfiguration(configuration);

            _serviceBusConnectionString = configuration[TenantConsts.ProvisionServiceConnectionString];
            _databaseCreatingQueue = configuration[TenantConsts.DatabaseCreatingQueue];
            _storageCreatingQueue = configuration[TenantConsts.StorageCreatingQueue];

            services.Configure<MwpServiceBusEventBusOptions>(options =>
            {
                options.ConnectionString = configuration[TenantConsts.ProvisionServiceConnectionString];

                options.Pulishers.AddIfNotContains(KeyValuePair.Create(typeof(DatabaseProvisionRequestEventData),
                    configuration[TenantConsts.DatabaseCreatingQueue]));
                options.Pulishers.AddIfNotContains(KeyValuePair.Create(typeof(StorageProvisionRequestEventData),
                    configuration[TenantConsts.StorageCreatingQueue]));
            });
        }

        [Fact(Skip = SkipReasonForManualRun)]
        public async Task ProvisionDatabase_ShouldSendProvisionMessage()
        {
            // arrange
            var resource = new TenantResource(Guid.NewGuid(), Guid.NewGuid())
            {
                Name = "myTenant",
                IsActive = false,
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)CloudServiceOptions.DatabaseStandard,
                ProvisionStatus = ProvisionStatus.Initial,
                ServerName = "TestServerName_Database",
                ElasticPoolName = "TestElasticPoolName",
                ResourceGroup = "TestResourceGroup_Database",
                SubscriptionId = "TestSubscriptionId_Database"
            };

            // act
            await _resourceProvisionRequestManager.ProvisionDatabase(resource);

            // assert
            await AssertProvisionMessage<DatabaseProvisionRequestEventData>(resource, _databaseCreatingQueue);
        }

        [Fact(Skip = SkipReasonForManualRun)]
        public async Task ProvisionStorage_ShouldSendProvisionMessage()
        {
            // arrange
            var resource = new TenantResource(Guid.NewGuid(), Guid.NewGuid())
            {
                IsActive = false,
                CloudServiceLocationId = (int)CloudServiceLocations.AustraliaEast,
                CloudServiceOptionId = (int)CloudServiceOptions.StoragePremium,
                ProvisionStatus = ProvisionStatus.Initial,
                ServerName = "TestServerName_Storage",
                ResourceGroup = "TestResourceGroup_Storage",
                SubscriptionId = "TestSubscriptionId_Storage",
                Name = "TestStorageName"
            };

            // act
            await _resourceProvisionRequestManager.ProvisionStorage(resource);

            // assert
            await AssertProvisionMessage<StorageProvisionRequestEventData>(resource, _storageCreatingQueue);
        }

        #region Private methods

        private async Task AssertProvisionMessage<TEventData>(TenantResource resource, string queueName)
            where TEventData : ResourceProvisionRequestEventData
        {
            var receiver = new MessageReceiver(_serviceBusConnectionString, queueName, ReceiveMode.ReceiveAndDelete);
            try
            {
                var message = await receiver.PeekAsync();
                var body = Encoding.UTF8.GetString(message.Body);

                var result = JsonConvert.DeserializeObject<TEventData>(body);
                result.TenantId.ShouldBe(resource.TenantId);
                result.LocationId.ShouldBe(resource.CloudServiceLocationId);
                result.CloudServiceOptionId.ShouldBe(resource.CloudServiceOptionId);
                result.ServerName.ShouldBe(resource.ServerName);
                result.ResourceGroupName.ShouldBe(resource.ResourceGroup);
                result.SubscriptionId.ShouldBe(resource.SubscriptionId);

                if (typeof(TEventData) == typeof(DatabaseProvisionRequestEventData))
                {
                    var databaseRequestMessage = result as DatabaseProvisionRequestEventData;
                    databaseRequestMessage?.ElasticPoolName.ShouldBe(resource.ElasticPoolName);
                    databaseRequestMessage?.DatabaseName.ShouldBe(resource.Name);
                }
            }
            finally
            {
                // Clear message from queue to prevent duplication
                await receiver.ReceiveAsync();
                await receiver.CloseAsync();
            }
        }

        #endregion Private methods
    }
}