using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mwp.Data;
using Volo.Abp.DependencyInjection;

namespace Mwp.EntityFrameworkCore
{
    public class EntityFrameworkCoreMwpDbSchemaMigrator
        : IMwpDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMwpRollbackHelper _mwpRollbackHelper;

        public EntityFrameworkCoreMwpDbSchemaMigrator(IServiceProvider serviceProvider, IMwpRollbackHelper mwpRollbackHelper)
        {
            _serviceProvider = serviceProvider;
            _mwpRollbackHelper = mwpRollbackHelper;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the MwpMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */
            var dbContext = _serviceProvider.GetRequiredService<MwpMigrationsDbContext>();
            var connectionString = dbContext.Database.GetDbConnection()?.ConnectionString;

            if (!string.IsNullOrEmpty(connectionString) && connectionString != "Data Source=:memory:")
            {
                await dbContext
                    .Database
                    .MigrateAsync();
            }
        }

        public async Task RollbackAsync(string script = null)
        {
            var rollbackScript = script;
            if (string.IsNullOrEmpty(script))
            {
                var latestMigration = (await _serviceProvider
                        .GetRequiredService<MwpMigrationsDbContext>()
                        .Database
                        .GetAppliedMigrationsAsync())
                    .LastOrDefault();

                rollbackScript = _mwpRollbackHelper.GetRollbackScript(latestMigration);
            }

            await _serviceProvider
                .GetRequiredService<MwpMigrationsDbContext>()
                .Database
                .ExecuteSqlRawAsync(rollbackScript);
        }
    }
}