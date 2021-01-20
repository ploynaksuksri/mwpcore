using System.Threading.Tasks;
using Microsoft.Azure.Management.Storage.Fluent;
using Mwp.CloudService;

namespace Mwp.Storage
{
    public interface IStorageManager
    {
        Task<bool> CheckAccountNameAvailability(string accountName);

        Task<IStorageAccount> CreateAsync(string resourceGroupName, string accountName, CloudServiceLocations region);

        Task DeleteAsync(string resourceGroupName, string accountName);

        Task<IStorageAccount> GetAsync(string resourceGroupName, string accountName);

        Task<string> GetConnectionString(string resourceGroupName, string accountName);
    }
}