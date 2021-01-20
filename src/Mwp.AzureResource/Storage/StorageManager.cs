using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Extensions.Configuration;
using Mwp.CloudService;
using Volo.Abp.DependencyInjection;

namespace Mwp.Storage
{
    public class StorageManager : AzureResourceManager, IStorageManager, ITransientDependency
    {
        public StorageManager(IConfiguration config) : base(config)
        {
        }

        public async Task<bool> CheckAccountNameAvailability(string accountName)
        {
            var result = await Azure.StorageAccounts.CheckNameAvailabilityAsync(accountName);
            return result.IsAvailable ?? true;
        }

        public async Task<IStorageAccount> CreateAsync(string resourceGroupName, string accountName, CloudServiceLocations region)
        {
            var sdkRegion = MapRegion(region);
            var storage = await Azure.StorageAccounts.Define(accountName)
                .WithRegion(sdkRegion)
                .WithExistingResourceGroup(resourceGroupName)
                .WithAccessFromAzureServices()
                .WithSku(StorageAccountSkuType.Standard_GRS)
                .WithTag("createdBy", "SDK")
                .CreateAsync();
            return storage;
        }

        public async Task<IStorageAccount> GetAsync(string resourceGroupName, string accountName)
        {
            return await Azure.StorageAccounts.GetByResourceGroupAsync(resourceGroupName, accountName);
        }

        public async Task DeleteAsync(string resourceGroupName, string accountName)
        {
            await Azure.StorageAccounts.DeleteByResourceGroupAsync(resourceGroupName, accountName);
        }

        public async Task<string> GetConnectionString(string resourceGroupName, string accountName)
        {
            var storage = await GetAsync(resourceGroupName, accountName);
            return BuildConnectionString(accountName, await GetAccessKey(storage));
        }

        #region private methods

        private async Task<string> GetAccessKey(IStorageAccount storage)
        {
            var keys = await storage.GetKeysAsync();

            var key1 = keys.First(e => e.KeyName == "key1");
            return key1.Value;
        }

        private string BuildConnectionString(string name, string key)
        {
            return TenantResourceConsts.StorageConnectionStringTemplate
                .Replace("{name}", name)
                .Replace("{key}", key);
        }

        #endregion private methods
    }
}