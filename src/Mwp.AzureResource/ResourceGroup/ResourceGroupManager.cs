using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Extensions.Configuration;
using Mwp.CloudService;
using Volo.Abp.DependencyInjection;

namespace Mwp.ResourceGroup
{
    public class ResourceGroupManager : AzureResourceManager, IResourceGroupManager, ITransientDependency
    {
        public ResourceGroupManager(IConfiguration config) : base(config)
        {
        }

        public async Task<IResourceGroup> CreateAsync(string name, CloudServiceLocations region)
        {
            return await Azure.ResourceGroups.Define(name).WithRegion(MapRegion(region)).CreateAsync();
        }

        public IResourceGroup Create(string name, CloudServiceLocations region)
        {
            return Azure.ResourceGroups.Define(name).WithRegion(MapRegion(region)).Create();
        }

        public async Task DeleteAsync(string name)
        {
            await Azure.ResourceGroups.DeleteByNameAsync(name);
        }
    }
}