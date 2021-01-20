using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Mwp.CloudService;

namespace Mwp.ResourceGroup
{
    public interface IResourceGroupManager
    {
        Task<IResourceGroup> CreateAsync(string name, CloudServiceLocations region);

        IResourceGroup Create(string name, CloudServiceLocations region);

        Task DeleteAsync(string name);
    }
}