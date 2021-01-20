using System.Collections.Generic;
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
    public class MwpTenantMigrationService : ITransientDependency
    {
        private readonly ILogger<MwpTenantMigrationService> _logger;
        private readonly IDataSeeder _dataSeeder;
        private readonly ITenantExRepository _tenantExRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly IEnumerable<IMwpDbSchemaMigrator> _dbSchemaMigrators;

        public MwpTenantMigrationService(IDataSeeder dataSeeder,
            IEnumerable<IMwpDbSchemaMigrator> dbSchemaMigrators,
            ITenantExRepository tenantExRepository,
            ICurrentTenant currentTenant)
        {
            _dataSeeder = dataSeeder;
            _dbSchemaMigrators = dbSchemaMigrators;
            _tenantExRepository = tenantExRepository;
            _currentTenant = currentTenant;

            _logger = NullLogger<MwpTenantMigrationService>.Instance;
        }

        public async Task InitialiseTenantDatabaseAsync(Tenant tenant, string adminEmailAddress, string adminPassword)
        {
            using (_currentTenant.Change(tenant.Id))
            {
                if (await _tenantExRepository.IsSchemaExist())
                {
                    _logger.LogInformation($"Database schema for {tenant.Name} have been initialized.");
                    _logger.LogInformation($"Add admin user {adminEmailAddress}.");
                    await SeedDataAsync(tenant, adminEmailAddress, adminPassword);
                }
                else
                {
                    _logger.LogInformation($"Initialise {tenant.Name} database schema...");
                    await MigrateDatabaseSchemaAsync(tenant);
                    await SeedDataAsync(tenant, adminEmailAddress, adminPassword);
                    _logger.LogInformation($"Successfully completed {tenant.Name} database migrations.");
                }
            }
        }

        private async Task MigrateDatabaseSchemaAsync(Tenant tenant)
        {
            _logger.LogInformation($"Migrating schema for {(tenant == null ? "host" : tenant.Name + " tenant")} database...");

            foreach (var migrator in _dbSchemaMigrators)
            {
                await migrator.MigrateAsync();
            }
        }

        private async Task SeedDataAsync(Tenant tenant, string adminEmailAddress, string adminPassword)
        {
            _logger.LogInformation($"Executing {(tenant == null ? "host" : tenant.Name + " tenant")} database seed...");

            var context = new DataSeedContext(tenant?.Id)
            {
                [TenantConsts.AdminEmailProperty] = adminEmailAddress ?? TenantConsts.DefaultAdminEmail,
                [TenantConsts.AdminPasswordProperty] = adminPassword
            };

            await _dataSeeder.SeedAsync(context);
        }
    }
}
