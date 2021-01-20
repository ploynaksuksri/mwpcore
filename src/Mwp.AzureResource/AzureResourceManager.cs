using System.Collections.Generic;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Mwp.CloudService;

namespace Mwp
{
    public abstract class AzureResourceManager
    {
        protected IAzure Azure;
        protected string DefaultUserId;
        protected string DefaultPassword;

        protected static Dictionary<CloudServiceLocations, Region> RegionMap = new Dictionary<CloudServiceLocations, Region>
        {
            { CloudServiceLocations.AustraliaEast, Region.AustraliaEast },
            { CloudServiceLocations.AustraliaSoutheast, Region.AustraliaSouthEast },
            { CloudServiceLocations.SoutheastAsia, Region.AsiaSouthEast },
            { CloudServiceLocations.UKSouth, Region.UKSouth },
            { CloudServiceLocations.UKWest, Region.UKWest }
        };

        protected AzureResourceManager(IConfiguration configuration)
        {
            DefaultUserId = configuration[TenantResourceConsts.DefaultUserIdSetting];
            DefaultPassword = configuration[TenantResourceConsts.DefaultPasswordSetting];

            var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(
                configuration[TenantResourceConsts.AzureClientId],
                configuration[TenantResourceConsts.AzureClientSecret],
                configuration[TenantResourceConsts.AzureTenantId],
                AzureEnvironment.AzureGlobalCloud);

            Azure = Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .Authenticate(credentials)
                .WithDefaultSubscription();
        }

        protected Region MapRegion(CloudServiceLocations azureRegion)
        {
            if (RegionMap.TryGetValue(azureRegion, out var region))
            {
                return region;
            }

            return Region.AustraliaEast;
        }
    }
}