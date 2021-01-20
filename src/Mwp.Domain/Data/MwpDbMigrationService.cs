using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mwp.Tenants;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Saas.Tenants;

namespace Mwp.Data
{
    public class MwpDbMigrationService : ITransientDependency
    {
        public ILogger<MwpDbMigrationService> Logger { get; set; }

        private readonly IDataSeeder _dataSeeder;
        private readonly IEnumerable<IMwpDbSchemaMigrator> _dbSchemaMigrators;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantExRepository _tenantExRepository;
        private readonly ICurrentTenant _currentTenant;

        public MwpDbMigrationService(
            IDataSeeder dataSeeder,
            IEnumerable<IMwpDbSchemaMigrator> dbSchemaMigrators,
            ITenantRepository tenantRepository,
            ITenantExRepository tenantExRepository,
            ICurrentTenant currentTenant)
        {
            _dataSeeder = dataSeeder;
            _dbSchemaMigrators = dbSchemaMigrators;
            _tenantRepository = tenantRepository;
            _tenantExRepository = tenantExRepository;
            _currentTenant = currentTenant;

            Logger = NullLogger<MwpDbMigrationService>.Instance;
        }

        public async Task MigrateAsync(int? locationId, bool rollback)
        {
            Logger.LogInformation("Started database migrations...");

            if (rollback)
            {
                await RollbackDatabaseSchemaAsync();
            }
            else
            {
                await MigrateDatabaseSchemaAsync();
            }

            Logger.LogInformation($"Successfully completed host database migrations.");

            var tenants = await GetTenants(locationId);

            var migratedDatabaseSchemas = new HashSet<string>();
            foreach (var tenant in tenants)
            {
                using (_currentTenant.Change(tenant.Id))
                {
                    if (tenant.ConnectionStrings.Any())
                    {
                        var tenantConnectionStrings = tenant.ConnectionStrings
                            .Select(x => x.Value)
                            .ToList();

                        if (!migratedDatabaseSchemas.IsSupersetOf(tenantConnectionStrings))
                        {
                            if (rollback)
                            {
                                await RollbackDatabaseSchemaAsync(tenant);
                            }
                            else
                            {
                                await MigrateDatabaseSchemaAsync(tenant);
                            }

                            migratedDatabaseSchemas.AddIfNotContains(tenantConnectionStrings);
                        }
                    }

                    Logger.LogInformation($"Successfully completed {tenant.Name} tenant database migrations.");
                }

                Logger.LogInformation("Successfully completed database migrations.");
            }
        }

        private async Task MigrateDatabaseSchemaAsync(Tenant tenant = null)
        {
            Logger.LogInformation(
                $"Migrating schema for {(tenant == null ? "host" : tenant.Name + " tenant")} database...");

            foreach (var migrator in _dbSchemaMigrators)
            {
                await migrator.MigrateAsync();
            }

            await SeedDataAsync(tenant);
        }

        private async Task SeedDataAsync(Tenant tenant = null)
        {
            Logger.LogInformation($"Executing {(tenant == null ? "host" : tenant.Name + " tenant")} database seed...");

            await _dataSeeder.SeedAsync(tenant?.Id);
        }

        private async Task RollbackDatabaseSchemaAsync(Tenant tenant = null)
        {
            Logger.LogInformation($"Rolling back schema for {(tenant == null ? "host" : tenant.Name + " tenant")} database...");

            foreach (var migrator in _dbSchemaMigrators)
            {
                await migrator.RollbackAsync();
            }
        }

        private async Task<List<Tenant>> GetTenants(int? locationId)
        {
            if (locationId.HasValue && locationId > 0)
            {
                return await _tenantExRepository.GetTenantListByLocationIdAsync(locationId.Value);
            }

            return await _tenantRepository.GetListAsync(includeDetails: true);
        }
    }
}
