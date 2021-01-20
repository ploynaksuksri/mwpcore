using System.Threading.Tasks;
using Microsoft.Azure.Management.Sql.Fluent;
using Mwp.CloudService;

namespace Mwp.SqlDatabase
{
    public interface IDatabaseManager
    {
        Task<ISqlDatabase> CreateDatabase(string resourceGroupName, string serverName, string databaseName);

        Task<ISqlDatabase> AddDatabaseToPool(string resourceGroupName, string serverName, string elasticPoolName, string databaseName);

        Task<ISqlDatabase> CreateDatabase(ISqlServer sqlServer, string databaseName);

        Task<ISqlDatabase> GetDatabase(string resourceGroupName, string serverName, string databaseName);

        Task<ISqlServer> CreateSqlServer(string resourceGroupName, string serverName, CloudServiceLocations region);

        Task<ISqlServer> GetSqlServer(string resourceGroupName, string serverName);

        Task<ISqlElasticPool> CreateElasticPool(string resourceGroupName, string serverName, string elasticPoolName);

        Task<ISqlElasticPool> GetElasticPool(string resourceGroupName, string serverName, string poolName);

        Task<bool> CheckServerNameAvailabilityAsync(string serverName);

        Task DeleteAsync(string resourceGroupName, string serverName);
    }
}