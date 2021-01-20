using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Sql.Fluent;
using Microsoft.Extensions.Configuration;
using Mwp.CloudService;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Mwp.SqlDatabase
{
    public class DatabaseManager : AzureResourceManager, ITransientDependency, IDatabaseManager
    {
        public DatabaseManager(IConfiguration config) : base(config)
        {
        }

        public async Task<bool> CheckServerNameAvailabilityAsync(string serverName)
        {
            var result = await Azure.SqlServers.CheckNameAvailabilityAsync(serverName);
            return result.IsAvailable;
        }

        public async Task<ISqlDatabase> CreateDatabase(string resourceGroupName, string serverName, string databaseName)
        {
            var sqlServer = await GetSqlServer(resourceGroupName, serverName);
            return await sqlServer.Databases.Define(databaseName).CreateAsync();
        }

        public async Task<ISqlDatabase> CreateDatabase(ISqlServer sqlServer, string databaseName)
        {
            Check.NotNull(sqlServer, nameof(sqlServer));
            return await sqlServer.Databases.Define(databaseName).CreateAsync();
        }

        public async Task<ISqlDatabase> GetDatabase(string resourceGroupName, string serverName, string databaseName)
        {
            return await Azure.SqlServers.Databases.GetBySqlServerAsync(resourceGroupName, serverName, databaseName);
        }

        public async Task<ISqlDatabase> AddDatabaseToPool(string resourceGroupName, string serverName, string elasticPoolName, string databaseName)
        {
            var elasticPool = await GetElasticPool(resourceGroupName, serverName, elasticPoolName);
            return elasticPool.AddExistingDatabase(databaseName);
        }

        public async Task<ISqlServer> CreateSqlServer(string resourceGroupName, string serverName, CloudServiceLocations region)
        {
            var sdkRegion = MapRegion(region);
            return await Azure.SqlServers.Define(serverName)
                .WithRegion(sdkRegion)
                .WithExistingResourceGroup(resourceGroupName)
                .WithAdministratorLogin(DefaultUserId)
                .WithAdministratorPassword(DefaultPassword)
                .WithTag("createdBy", "SDK")
                .CreateAsync();
        }

        public async Task<ISqlServer> GetSqlServer(string resourceGroupName, string serverName)
        {
            return await Azure.SqlServers.GetByResourceGroupAsync(resourceGroupName, serverName);
        }

        public async Task<ISqlElasticPool> CreateElasticPool(string resourceGroupName, string serverName, string elasticPoolName)
        {
            var sqlServer = await Azure.SqlServers.GetByResourceGroupAsync(resourceGroupName, serverName);
            return await Azure.SqlServers.ElasticPools.Define(elasticPoolName)
                .WithExistingSqlServer(sqlServer)
                .WithBasicPool()
                .CreateAsync();
        }

        public async Task<ISqlElasticPool> GetElasticPool(string resourceGroupName, string serverName, string poolName)
        {
            return await Azure.SqlServers.ElasticPools.GetBySqlServerAsync(resourceGroupName, serverName, poolName);
        }

        public async Task DeleteAsync(string resourceGroupName, string serverName)
        {
            var sqlServer = await GetSqlServer(resourceGroupName, serverName);
            var databases = await sqlServer.Databases.ListAsync();
            foreach (var database in databases.Where(e => e.Name != "master"))
            {
                await sqlServer.Databases.DeleteAsync(database.Name);
            }

            await Azure.SqlServers.DeleteByResourceGroupAsync(resourceGroupName, serverName);
        }
    }
}